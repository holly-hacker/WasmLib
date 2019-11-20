using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Rivers;
using Rivers.Analysis;
using Rivers.Serialization.Dot;
using WasmLib.Decompilation.Intermediate;
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
            }
            
            Debug.Assert(!graph.IsCyclic(), "Got cyclic dependency in function!");
            
            // TODO: get all nodes with no outgoing edges, write them out (taking into account side effects?)
            // TODO: remove trees with no side effects

            var writer = new DotWriter(output, new DefaultUserDataSerializer());
            writer.Write(graph);
        }
    }
}