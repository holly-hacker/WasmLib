namespace WasmLib.FileFormat.Instructions
{
    public partial struct Instruction
    {
        public static OperandKind GetOperandKind(OpCode instr)
        {
            switch (instr) {
                case OpCode.Unreachable:
                case OpCode.Nop:
                    return OperandKind.None;

                case OpCode.Block: // these are special opcodes that serve as control flow markers
                case OpCode.Loop:
                case OpCode.If:
                    return OperandKind.BlockType;
                case OpCode.Else:
                case OpCode.End:
                    return OperandKind.None;

                case OpCode.Br:
                case OpCode.BrIf:
                    return OperandKind.LabelIndex;
                case OpCode.BrTable:
                    return OperandKind.BrTableOperand;

                case OpCode.Return:
                    return OperandKind.None;
                case OpCode.Call:
                    return OperandKind.FuncIndex;
                case OpCode.CallIndirect:
                    return OperandKind.IndirectCallTypeIndex;

                case OpCode.Drop:
                case OpCode.Select:
                    return OperandKind.None;

                case OpCode.LocalGet:
                case OpCode.LocalSet:
                case OpCode.LocalTee:
                    return OperandKind.LocalIndex;
                case OpCode.GlobalGet:
                case OpCode.GlobalSet:
                    return OperandKind.GlobalIndex;

                case OpCode.I32Load:
                case OpCode.I64Load:
                case OpCode.F32Load:
                case OpCode.F64Load:
                case OpCode.I32Load8S:
                case OpCode.I32Load8U:
                case OpCode.I32Load16S:
                case OpCode.I32Load16U:
                case OpCode.I64Load8S:
                case OpCode.I64Load8U:
                case OpCode.I64Load16S:
                case OpCode.I64Load16U:
                case OpCode.I64Load32S:
                case OpCode.I64Load32U:
                case OpCode.I32Store:
                case OpCode.I64Store:
                case OpCode.F32Store:
                case OpCode.F64Store:
                case OpCode.I32Store8:
                case OpCode.I32Store16:
                case OpCode.I64Store8:
                case OpCode.I64Store16:
                case OpCode.I64Store32:
                    return OperandKind.MemArg;

                case OpCode.MemorySize:
                case OpCode.MemoryGrow:
                    return OperandKind.Zero;

                case OpCode.I32Const:
                    return OperandKind.ImmediateI32;
                case OpCode.I64Const:
                    return OperandKind.ImmediateI64;
                case OpCode.F32Const:
                    return OperandKind.ImmediateF32;
                case OpCode.F64Const:
                    return OperandKind.ImmediateF64;

                default: // anything after 0x45 does not have an operand
                    return OperandKind.None;
            }
        }
    }
}