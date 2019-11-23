using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WasmLib.Decompilation.Intermediate;
using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib.Decompilation
{
    public class IntermediateRepresentationDecompiler : IDecompiler
    {
        public WasmModule WasmModule { get; }

        public IntermediateRepresentationDecompiler(WasmModule wasmModule)
        {
            WasmModule = wasmModule;
        }

        public void DecompileFunction(StreamWriter output, int functionIndex)
        {
            FunctionBody body = WasmModule.FunctionBodies[functionIndex];
            FunctionSignature signature = WasmModule.FunctionTypes[WasmModule.Functions[functionIndex]];
            
            // get IR
            var context = new IntermediateContext(signature, WasmModule, output);
            List<IntermediateInstruction> instructions = new IntermediateConverter(WasmModule, body, signature).Convert();

            output.Write(signature.ToString($"fun_{functionIndex:X8}"));
            output.WriteLine(" {");

            // write all IR while simulating the stack
            foreach (IntermediateInstruction instruction in instructions) {
                HandleInstruction(ref context, instruction);
            }
            
            output.WriteLine("}");

            if (context.Indentation != 0) {
                throw new Exception("Function body has unbalanced indentation");
            }

            if (context.Stack.Count != 0) {
                throw new Exception($"Unbalanced stack, found {context.Stack.Count} remaining values");
            }
            
            output.WriteLine();
        }

        private static void HandleInstruction(ref IntermediateContext context, IntermediateInstruction instruction)
        {
            if (context.RestOfBlockUnreachable && instruction.IsImplicit) {
                #if DEBUG
                context.WriteFull("// omitted implicit instruction because rest of block is unreachable");
                #endif
                return;
            }

            var args = new Variable[instruction.PopCount];
            
            for (int i = 0; i < instruction.PopCount; i++) {
                args[i] = context.Pop();
                Debug.Assert(instruction.PopTypes[i] == args[i].Type || instruction.PopTypes[i] == ValueKind.Any || args[i].Type == ValueKind.Any);
            }
            
            context.RestOfBlockUnreachable = instruction.RestOfBlockUnreachable;
            
            // NOTE: really ugly and slow, but can't be replaced with string.format since input is dynamic and can contain {}
            string s = instruction.OperationStringFormat.SafeFormat(args);
            
            Debug.Assert(instruction.PushCount <= 1);
            if (instruction.PushCount > 0) {
                s = $"{context.Push(instruction.PushTypes[0])} = {s}";
            }

            if (instruction.HasBlock) {
                s += " {";
            }

            if (instruction.Comment != null) {
                s += " // " + instruction.Comment;
            }
            
            context.WriteFull(s);

            if (instruction.HasBlock) {
                HandleBlock(ref context, instruction.Block1!);

                if (instruction.Block2 != null) {
                    context.WriteFull("} else {");
                    HandleBlock(ref context, instruction.Block2);
                }

                context.WriteFull("}");
            }

            static void HandleBlock(ref IntermediateContext context2, in ControlBlock block)
            {
                context2.EnterBlock();
            
                foreach (IntermediateInstruction instr in block.Instructions) {
                    HandleInstruction(ref context2, instr);
                }

                Debug.Assert(!(block.HasReturn && !context2.RestOfBlockUnreachable));

                context2.ExitBlock();
            }
        }
    }
}