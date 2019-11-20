using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class MemorySizeInstruction : IntermediateInstruction
    {
        public OperationKind Operation { get; }
        public override bool IsPure => false;

        public MemorySizeInstruction(in Instruction instruction)
        {
            Operation = instruction.OpCode switch {
                OpCode.MemorySize => OperationKind.Size,
                OpCode.MemoryGrow => OperationKind.Grow,
                _ => throw new WrongInstructionPassedException(instruction, nameof(MemorySizeInstruction)),
            };
        }

        public override ValueKind[] PopTypes => Operation == OperationKind.Grow ? new[] {ValueKind.I32} : new ValueKind[0];
        public override ValueKind[] PushTypes => new[] {ValueKind.I32};

        protected override string OperationStringFormat => Operation == OperationKind.Size ? "{0} = MEMORY.SIZE / PAGE_SIZE" : "{0} = MEMORY.GROW({1} * PAGE_SIZE)";

        public override string ToString() => Operation.ToString();

        public enum OperationKind
        {
            Size,
            Grow,
        }
    }
}