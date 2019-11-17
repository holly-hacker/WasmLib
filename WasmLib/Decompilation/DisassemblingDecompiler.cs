using System;
using System.IO;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Decompilation
{
    public class DisassemblingDecompiler : IDecompiler
    {
        public WasmModule WasmModule { get; private set; }

        public DisassemblingDecompiler(WasmModule wasmModule)
        {
            WasmModule = wasmModule;
        }

        public void DecompileFunction(StreamWriter output, int functionIndex)
        {
            FunctionBody body = WasmModule.FunctionBodies[functionIndex];
            FunctionSignature signature = WasmModule.FunctionTypes[WasmModule.Functions[functionIndex]];
            int indent = 1;

            output.WriteLine($"fun_{functionIndex:X8}: # {signature}");
            foreach (var instruction in body.Instructions)
            {
                indent += instruction.OpCode switch {
                    OpCode.Else => -1, // temporary
                    OpCode.End => -1,
                    _ => 0,
                };
                
                output.WriteLine(new string('\t', indent) + instruction);
                
                indent += instruction.OpCode switch {
                    OpCode.Block => 1,
                    OpCode.Loop => 1,
                    OpCode.If => 1,
                    OpCode.Else => 1,
                    _ => 0,
                };
            }

            // At the end, expect to be 1 indent level lower due to trailing `end` instruction
            if (indent != 0) {
                throw new Exception("Function body contains unbalanced branching instructions");
            }
            
            output.WriteLine();
        }
    }
}