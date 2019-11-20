using System;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class ConstInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public ulong RawOperand { get; }
        public override bool IsPure => true;
        
        public ConstInstruction(in Instruction instruction)
        {
            Type = instruction.OpCode switch {
                OpCode.I32Const => ValueKind.I32,
                OpCode.I64Const => ValueKind.I64,
                OpCode.F32Const => ValueKind.F32,
                OpCode.F64Const => ValueKind.F64,
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
            RawOperand = instruction.ULongOperand;
        }

        public override ValueKind[] PopTypes => new ValueKind[0];
        public override ValueKind[] PushTypes => new[] {Type};

        protected override string OperationStringFormat => "{0} = " + OperandString;

        private string OperandString => Type switch {
            ValueKind.I32 => $"0x{(uint)RawOperand:X}",
            ValueKind.I64 => $"0x{RawOperand:X}",
            ValueKind.F32 => $"{BitConverter.Int32BitsToSingle((int)RawOperand)}",
            ValueKind.F64 => $"{BitConverter.Int64BitsToDouble((long)RawOperand)}",
            _ => throw new ArgumentOutOfRangeException(),
        };

        public override string ToString()
        {
            return $"{OperandString}";
        }
    }
}