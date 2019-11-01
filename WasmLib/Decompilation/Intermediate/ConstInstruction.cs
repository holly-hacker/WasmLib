using System;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class ConstInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public ulong RawOperand { get; }
        
        public ConstInstruction(Instruction instruction)
        {
            Type = instruction.OpCode switch {
                InstructionKind.I32Const => ValueKind.I32,
                InstructionKind.I64Const => ValueKind.I64,
                InstructionKind.F32Const => ValueKind.F32,
                InstructionKind.F64Const => ValueKind.F64,
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
            RawOperand = instruction.ULongOperand;
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            var pushed = context.Push(Type);

            switch (Type) {
                case ValueKind.I32:
                    context.WriteFull($"{pushed} = 0x{(uint)RawOperand:X}");
                    break;
                case ValueKind.I64:
                    context.WriteFull($"{pushed} = 0x{RawOperand:X}");
                    break;
                case ValueKind.F32:
                    context.WriteFull($"{pushed} = {BitConverter.Int32BitsToSingle((int)RawOperand)}");
                    break;
                case ValueKind.F64:
                    context.WriteFull($"{pushed} = {BitConverter.Int64BitsToDouble((long)RawOperand)}");
                    break;
            }
        }
    }
}