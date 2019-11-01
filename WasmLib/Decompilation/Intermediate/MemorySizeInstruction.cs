using System.Diagnostics;
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
            Kind = instruction.Opcode switch {
                InstructionKind.MemorySize => OpCodeKind.Size,
                InstructionKind.MemoryGrow => OpCodeKind.Grow,
                _ => throw new WrongInstructionPassedException(instruction, nameof(MemorySizeInstruction)),
            };
        }

        public override void Handle(ref IntermediateContext context)
        {
            if (Kind == OpCodeKind.Size) {
                Variable pushed = context.Push(ValueKind.I32);
                context.WriteFull($"{pushed} = MEMORY.SIZE / PAGE_SIZE");
            }
            else if (Kind == OpCodeKind.Grow) {
                Variable popped = context.Pop();
                Debug.Assert(popped.Type == ValueKind.I32);
                Variable pushed = context.Push(ValueKind.I32);
                context.WriteFull($"{pushed} = MEMORY.GROW({popped} * PAGE_SIZE)");
            }
        }

        public enum OpCodeKind
        {
            Size,
            Grow,
        }
    }
}