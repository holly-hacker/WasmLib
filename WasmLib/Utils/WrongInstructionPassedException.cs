using System;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Utils
{
    internal class WrongInstructionPassedException : Exception
    {
        public WrongInstructionPassedException(Instruction instruction, string typeName) : base($"Passed incorrect instruction '{instruction}' to IR {typeName} constructor") { }
    }
}