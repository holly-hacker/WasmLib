using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class MemorySizeInstruction : IntermediateInstruction
    {
        public OpCodeKind Kind { get; }

        public MemorySizeInstruction(in Instruction instruction)
        {
            Kind = instruction.OpCode switch {
                OpCode.MemorySize => OpCodeKind.Size,
                OpCode.MemoryGrow => OpCodeKind.Grow,
                _ => throw new WrongInstructionPassedException(instruction, nameof(MemorySizeInstruction)),
            };
        }

        public override ValueKind[] PopTypes => Kind == OpCodeKind.Grow ? new[] {ValueKind.I32} : new ValueKind[0];
        public override ValueKind[] PushTypes => new[] {ValueKind.I32};

        protected override string OperationStringFormat => Kind == OpCodeKind.Size ? "{0} = MEMORY.SIZE / PAGE_SIZE" : "{0} = MEMORY.GROW({1} * PAGE_SIZE)";

        public enum OpCodeKind
        {
            Size,
            Grow,
        }
    }
}