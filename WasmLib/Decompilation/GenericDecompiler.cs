using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Rivers;
using Rivers.Serialization.Dot;
using WasmLib.Decompilation.Intermediate;
using WasmLib.Decompilation.Intermediate.Graph;
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
            // TODO: add return instruction to intermediate
            // TODO: add graph node for global state, locals or parameters?

            var graph = new Graph();
            var stack = new Stack<(InstructionNode, ValueKind)>();
            
            int instructionNum = 0;
            foreach (IntermediateInstruction instruction in instructions) {
                if (instruction.HasBlock) {
                    throw new NotImplementedException();
                }

                InstructionNode node = new InstructionNode(instruction, instructionNum++);
                graph.Nodes.Add(node);

                for (int i = 0; i < instruction.PopCount; i++) {
                    (InstructionNode sourceInstruction, ValueKind type) = stack.Pop();
                    Debug.Assert(type == instruction.PopTypes[i]);
                    sourceInstruction.OutgoingEdges.Add(node);
                }
                for (int i = 0; i < instruction.PushCount; i++) {
                    stack.Push((node, instruction.PushTypes[i]));
                }
                
                // if this instruction is not pure, add a dependency on the last impure instruction, if any
                // NOTE: this could possibly be optimized by having different kinds of impurity
                if (!instruction.IsPure) {
                    InstructionNode? dependentInstruction = graph.Nodes.Cast<InstructionNode>().Reverse().Skip(1).FirstOrDefault(x => !x.Instruction.IsPure);
                    dependentInstruction?.OutgoingEdges.Add(node);
                }
            }
            
            // this assert seems to fail, perhaps write own version in the future
            // Debug.Assert(!graph.IsCyclic(), "Got cyclic dependency in function!");
            
            // TODO: get all nodes with no outgoing edges, write them out (taking into account side effects?)
            // TODO: remove trees with no side effects

            var writer = new DotWriter(output, new DefaultUserDataSerializer());
            writer.Write(graph);
        }
    }
}