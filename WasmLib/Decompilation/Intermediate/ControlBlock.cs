using System.Collections.Generic;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public class ControlBlock
    {
        public IReadOnlyList<IntermediateInstruction> Instructions { get; }
        public ValueKind ValueKind { get; }

        public bool HasReturn => ValueKind != ValueKind.Empty;

        public ControlBlock(IReadOnlyList<IntermediateInstruction> instructions, ValueKind valueKind)
        {
            Instructions = instructions;
            ValueKind = valueKind;
        }
    }
}