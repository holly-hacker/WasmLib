using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Rivers;
using Rivers.Analysis;
using Rivers.Serialization.Dot;
using WasmLib.Decompilation.Intermediate;
using WasmLib.Decompilation.Intermediate.Graph;
using WasmLib.Decompilation.SourceCode;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation
{
    public class GenericDecompiler : IDecompiler
    {
        public WasmModule WasmModule { get; }

        public GenericDecompiler(WasmModule wasmModule)
        {
            WasmModule = wasmModule;
        }
        
        public void DecompileFunction(StreamWriter output, int functionIndex)
        {
            FunctionBody body = WasmModule.FunctionBodies[functionIndex];
            FunctionSignature signature = WasmModule.FunctionTypes[WasmModule.Functions[functionIndex]];
            
            List<IntermediateInstruction> instructions = new IntermediateConverter(WasmModule, body, signature).Convert();

            Graph graph = CreateGraph(instructions);
            
            // TODO: remove subtrees with no side effects?

            output.Write(signature.ToString($"fun_{functionIndex:X8}"));
            output.WriteLine(" {");
            OutputAsCode(graph, output);
            output.WriteLine("}");
            output.WriteLine();
            
            // var writer = new DotWriter(output);
            // writer.Write(graph);
        }

        private static Graph CreateGraph(IEnumerable<IntermediateInstruction> instructions)
        {
            var graph = new Graph();
            var stack = new Stack<(InstructionNode, ValueKind)>();

            int instructionNum = 0;
            foreach (IntermediateInstruction instruction in instructions) {
                InstructionNode node = !instruction.HasBlock
                    ? new InstructionNode(instruction, instructionNum++)
                    : instruction.Block2 == null
                        ? new InstructionNode(instruction, instructionNum++, CreateGraph(instruction.Block1!.Instructions))
                        : new InstructionNode(instruction, instructionNum++, CreateGraph(instruction.Block1!.Instructions), CreateGraph(instruction.Block2.Instructions));
                
                graph.Nodes.Add(node);

                Debug.Assert(instruction.PushCount <= 1, "Instruction pushed multiple variables to stack, which shouldn't happen.");
                for (int i = 0; i < instruction.PopCount; i++) {
                    (InstructionNode sourceInstruction, ValueKind type) = stack.Pop();
                    Debug.Assert(type == instruction.PopTypes[i] || type == ValueKind.Any || instruction.PopTypes[i] == ValueKind.Any);
                    sourceInstruction.OutgoingEdges.Add(new StackVariableEdge(sourceInstruction, node, type));
                }

                for (int i = 0; i < instruction.PushCount; i++) {
                    stack.Push((node, instruction.PushTypes[i]));
                }

                // if this instruction is not pure, add a dependency on the last impure instruction, if any
                // NOTE: this could possibly be optimized by having different kinds of impurity
                // TODO: edges not required anymore for decompiled code output?
                if (node.IsOrderImportant) {
                    InstructionNode? dependentInstruction = graph.Nodes.Cast<InstructionNode>().Reverse().Skip(1).FirstOrDefault(x => x.IsOrderImportant);
                    dependentInstruction?.OutgoingEdges.Add(new ImpurityDependencyEdge(dependentInstruction, node));
                }
            }

            // BUG: see Washi1337/Rivers#6
            // Debug.Assert(!graph.IsCyclic(), "Got cyclic dependency in function!");
            if (!graph.IsConnected()) {
                #if DEBUG
                using (StreamWriter sw = new StreamWriter(File.OpenWrite("temp_unconnected.dot"))) new DotWriter(sw).Write(graph);
                #endif
                throw new NotImplementedException();
            }

            return graph;
        }

        private static void OutputAsCode(Graph graph, TextWriter output, int tabCount = 1, Dictionary<ValueKind, int>? varCounts = null)
        {
            varCounts ??= new Dictionary<ValueKind, int> {
                {ValueKind.I32, 0},
                {ValueKind.I64, 0},
                {ValueKind.F32, 0},
                {ValueKind.F64, 0},
            };
            
            var statements = new Dictionary<int, IExpression>();
            foreach (var currentNode in graph.Nodes.OfType<InstructionNode>()) {
                var parameterEdges = currentNode.IncomingVariableEdges.ToArray();

                // only handle if node consumes variables
                if (parameterEdges.Any()) {
                    // for each dependency, check if it can be reached in a pure way
                    // if so, inline it
                    // if not, create intermediary statements
                    var parameters = new IExpression[parameterEdges.Length];
                    for (int i = 0; i < parameterEdges.Length; i++) {
                        var edge = parameterEdges[i];
                        var variableNode = edge.Source;
                        var impureInstructions = graph.Nodes.OfType<InstructionNode>().Where(x => x.Index > variableNode.Index && x.Index < currentNode.Index && x.IsOrderImportant);

                        if (currentNode.Instruction.CanInline && variableNode.Instruction.CanBeInlined && !impureInstructions.Any()) {
                            parameters[i] = statements[variableNode.Index];
                            statements.Remove(variableNode.Index);
                        }
                        else {
                            var assignment = new AssignmentExpression(statements[variableNode.Index], edge.Type, varCounts[edge.Type]++);
                            statements[variableNode.Index] = assignment;
                            parameters[i] = assignment.Reference;
                        }
                    }

                    statements[currentNode.Index] = new GenericExpression(currentNode.Instruction, parameters, currentNode.Block1, currentNode.Block2);
                }
                else {
                    statements[currentNode.Index] = new GenericExpression(currentNode.Instruction, null, currentNode.Block1, currentNode.Block2);
                }
            }

            foreach (IExpression expression in statements.Values) {
                var stringRepresentation = expression.GetStringRepresentation();

                // don't write empty statements such as blocks
                if (string.IsNullOrEmpty(stringRepresentation)) {
                    continue;
                }
                
                // TODO: support comments
                if (expression is IHasBlocks blocks && blocks.Block1 != null) {
                    output.WriteLine(new string('\t', tabCount) + stringRepresentation + " {");
                    
                    OutputAsCode(blocks.Block1, output, tabCount + 1, varCounts);

                    if (blocks.Block2 == null) {
                        output.WriteLine(new string('\t', tabCount) + "}");
                    }
                    else {
                        output.WriteLine(new string('\t', tabCount) + "} else {");
                        OutputAsCode(blocks.Block2, output, tabCount + 1, varCounts);
                        output.WriteLine(new string('\t', tabCount) + "}");
                    }
                }
                else {
                    output.WriteLine(new string('\t', tabCount) + stringRepresentation);
                }
            }
        }
    }
}