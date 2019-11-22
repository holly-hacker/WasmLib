using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Rivers;
using Rivers.Analysis;
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
                if (instruction.HasBlock) {
                    throw new NotImplementedException();
                }

                InstructionNode node = new InstructionNode(instruction, instructionNum++);
                graph.Nodes.Add(node);

                Debug.Assert(instruction.PushCount <= 1, "Instruction pushed multiple variables to stack, which shouldn't happen.");
                for (int i = 0; i < instruction.PopCount; i++) {
                    (InstructionNode sourceInstruction, ValueKind type) = stack.Pop();
                    Debug.Assert(type == instruction.PopTypes[i]);
                    sourceInstruction.OutgoingEdges.Add(new StackVariableEdge(sourceInstruction, node, type));
                }

                for (int i = 0; i < instruction.PushCount; i++) {
                    stack.Push((node, instruction.PushTypes[i]));
                }

                // if this instruction is not pure, add a dependency on the last impure instruction, if any
                // NOTE: this could possibly be optimized by having different kinds of impurity
                // TODO: edges not required anymore for decompiled code output?
                if (!node.IsPure) {
                    InstructionNode? dependentInstruction = graph.Nodes.Cast<InstructionNode>().Reverse().Skip(1).FirstOrDefault(x => !x.IsPure);
                    dependentInstruction?.OutgoingEdges.Add(new ImpurityDependencyEdge(dependentInstruction, node));
                }
            }

            // BUG: see Washi1337/Rivers#6
            // Debug.Assert(!graph.IsCyclic(), "Got cyclic dependency in function!");
            if (!graph.IsConnected()) {
                throw new NotImplementedException();
            }

            return graph;
        }

        private static void OutputAsCode(Graph graph, TextWriter output)
        {
            var varCounts = new Dictionary<ValueKind, int> {
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
                        var isPure = !graph.Nodes.OfType<InstructionNode>().Any(x => !x.IsPure & x.Index > variableNode.Index && x.Index < currentNode.Index);

                        if (isPure) { // TODO: and instruction can be inlined
                            parameters[i] = statements[variableNode.Index];
                            statements.Remove(variableNode.Index);
                        }
                        else {
                            var assignment = new AssignmentExpression(statements[variableNode.Index], edge.Type, varCounts[edge.Type]++);
                            statements[variableNode.Index] = assignment;
                            parameters[i] = assignment.Reference;
                        }
                    }

                    statements[currentNode.Index] = new GenericExpression(currentNode.Instruction, parameters);
                }
                else {
                    statements[currentNode.Index] = new GenericExpression(currentNode.Instruction);
                }

            }

            foreach (IExpression expression in statements.Values) {
                // TODO: support comments
                output.WriteLine(new string('\t', 1) + expression.GetStringRepresentation());
            }
        }
    }
}