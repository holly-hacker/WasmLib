using System;
using System.IO;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Utils
{
    /// <remarks>
    /// TODO: move to a proper decompiler class
    /// </remarks>
    public class SimpleDecompiler
    {
        public WasmFile WasmFile { get; private set; }

        public SimpleDecompiler(WasmFile wasmFile)
        {
            WasmFile = wasmFile;
        }

        public void DecompileFunction(StreamWriter output, int functionIndex)
        {
            var function = WasmFile.FunctionBodies[functionIndex];
            int indent = 1;

            output.WriteLine($"fun_{functionIndex:X8}:");
            foreach (var instruction in function.Body)
            {
                indent += instruction.Opcode switch {
                    InstructionKind.Else => -1, // temporarily
                    InstructionKind.End => -1,
                    _ => 0,
                };
                
                output.WriteLine(new string('\t', indent) + instruction);
                
                indent += instruction.Opcode switch {
                    InstructionKind.Block => 1,
                    InstructionKind.Loop => 1,
                    InstructionKind.If => 1,
                    InstructionKind.Else => 1,
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