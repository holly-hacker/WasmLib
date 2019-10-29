using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Utils
{
    public static class Disassembler
    {
        public static IEnumerable<Instruction> Disassemble(BinaryReader br, uint length)
        {
            long startPos = br.BaseStream.Position;
            while (startPos + length > br.BaseStream.Position) {
                yield return DisassembleInstruction(br);
            }
        }

        public static IEnumerable<Instruction> DisassembleExpression(BinaryReader br)
        {
            Instruction instr;
            do {
                yield return instr = DisassembleInstruction(br);
            } while (instr.Opcode != InstructionKind.End);
        }

        public static Instruction DisassembleInstruction(BinaryReader br)
        {
            var opcode = (InstructionKind)br.ReadByte();

            switch (Instruction.GetOperandKind(opcode)) {
                case OperandKind.None:
                    return new Instruction(opcode);
                case OperandKind.BlockType:
                    return new Instruction(opcode, br.ReadVarUint7());
                case OperandKind.LabelIndex:
                case OperandKind.FuncIndex:
                case OperandKind.LocalIndex:
                case OperandKind.GlobalIndex:
                    return new Instruction(opcode, br.ReadVarUint32());
                case OperandKind.IndirectCallTypeIndex: // has a 0x00 after it
                    var instruction = new Instruction(opcode, br.ReadVarUint32());
                    byte trap = br.ReadByte();
                    Debug.Assert(trap == 0x00);
                    return instruction;
                case OperandKind.BrTableOperand:
                    return new Instruction(opcode, operandObject: br.ReadVarUint32Array(), operand: br.ReadVarUint32());
                case OperandKind.MemArg:
                    return new Instruction(opcode, br.ReadVarUint32() | ((ulong)br.ReadVarUint32() << 32));
                case OperandKind.Zero:
                    byte zero = br.ReadByte();
                    Debug.Assert(zero == 0x00);
                    return new Instruction(opcode);
                case OperandKind.ImmediateI32:
                    return new Instruction(opcode, br.ReadVarUint32());
                case OperandKind.ImmediateI64:
                    return new Instruction(opcode, br.ReadVarUint64());
                case OperandKind.ImmediateF32:
                    return new Instruction(opcode, (ulong)BitConverter.SingleToInt32Bits(br.ReadSingle()));
                case OperandKind.ImmediateF64:
                    return new Instruction(opcode, (ulong)BitConverter.DoubleToInt64Bits(br.ReadDouble()));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
