using System;
using System.Linq;

namespace WasmLib.FileFormat.Instructions
{
    public partial struct Instruction
    {
        public InstructionKind Opcode { get; private set; }
        private ulong operand;
        private object? operandObject;

        public Instruction(InstructionKind opcode)
        {
            Opcode = opcode;
            operand = 0;
            operandObject = null;
        }

        public Instruction(InstructionKind opcode, ulong operand)
        {
            Opcode = opcode;
            this.operand = operand;
            operandObject = null;
        }

        public Instruction(InstructionKind opcode, object operandObject)
        {
            Opcode = opcode;
            operand = 0;
            this.operandObject = operandObject;
        }

        public Instruction(InstructionKind opcode, ulong operand, object operandObject)
        {
            Opcode = opcode;
            this.operand = operand;
            this.operandObject = operandObject;
        }

        public override string ToString()
        {
            var opcodeString = GetOpcodeName(Opcode);
            
            switch (GetOperandKind(Opcode)) {
                case OperandKind.None:
                    return opcodeString;
                case OperandKind.BrTableOperand:
                    return $"{opcodeString} {string.Join("-", ((uint[])operandObject!).Select(x => $"0x{x:X}"))} 0x{operand:x}";
                case OperandKind.BlockType:
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
