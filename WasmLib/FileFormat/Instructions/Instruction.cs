using System;
using System.Linq;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Instructions
{
    public partial struct Instruction
    {
        public InstructionKind OpCode { get; private set; }

        private readonly ulong operand;
        private readonly object? operandObject; // NOTE: this is only ever a uint[], could be specified

        public uint UIntOperand => (uint)operand;
        public int IntOperand => (int)operand;
        public ulong ULongOperand => operand;
        public long LongOperand => (long)operand;

        public Instruction(InstructionKind opCode, ulong operand = 0, object? operandObject = null)
        {
            OpCode = opCode;
            this.operand = operand;
            this.operandObject = operandObject;
        }

        public override string ToString()
        {
            var opcodeString = EnumUtils.GetDescription(OpCode);
            
            switch (GetOperandKind(OpCode)) {
                case OperandKind.None:
                    return opcodeString;
                case OperandKind.BrTableOperand:
                    return $"{opcodeString} {string.Join("-", ((uint[])operandObject!).Select(x => $"0x{x:X}"))} 0x{operand:x}";
                case OperandKind.BlockType:
                    return $"{opcodeString} {(ValueKind)operand}";
                case OperandKind.LabelIndex:
                case OperandKind.FuncIndex:
                case OperandKind.IndirectCallTypeIndex:
                    return $"{opcodeString} 0x{operand:X}";
                case OperandKind.MemArg:
                    return $"{opcodeString} 0x{operand & 0xFFFFFFFF00000000:X} 0x{operand & 0xFFFFFFFF:X}";
                case OperandKind.Zero:
                case OperandKind.LocalIndex:
                case OperandKind.GlobalIndex:
                case OperandKind.ImmediateI32:
                case OperandKind.ImmediateI64:
                    return $"{opcodeString} {operand}";
                case OperandKind.ImmediateF32:
                    return $"{opcodeString} {BitConverter.Int32BitsToSingle((int)operand)}";
                case OperandKind.ImmediateF64:
                    return $"{opcodeString} {BitConverter.Int64BitsToDouble((long)operand)}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
