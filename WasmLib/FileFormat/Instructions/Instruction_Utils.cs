namespace WasmLib.FileFormat.Instructions
{
    public partial struct Instruction
    {
        public static OperandKind GetOperandKind(InstructionKind instr)
        {
            switch (instr) {
                case InstructionKind.Unreachable:
                case InstructionKind.Nop:
                    return OperandKind.None;

                case InstructionKind.Block: // these are special opcodes that serve as control flow markers
                case InstructionKind.Loop:
                case InstructionKind.If:
                    return OperandKind.BlockType;
                case InstructionKind.Else:
                case InstructionKind.End:
                    return OperandKind.None;

                case InstructionKind.Br:
                case InstructionKind.BrIf:
                    return OperandKind.LabelIndex;
                case InstructionKind.BrTable:
                    return OperandKind.BrTableOperand;

                case InstructionKind.Return:
                    return OperandKind.None;
                case InstructionKind.Call:
                    return OperandKind.FuncIndex;
                case InstructionKind.CallIndirect:
                    return OperandKind.IndirectCallTypeIndex;

                case InstructionKind.Drop:
                case InstructionKind.Select:
                    return OperandKind.None;

                case InstructionKind.LocalGet:
                case InstructionKind.LocalSet:
                case InstructionKind.LocalTee:
                    return OperandKind.LocalIndex;
                case InstructionKind.GlobalGet:
                case InstructionKind.GlobalSet:
                    return OperandKind.GlobalIndex;

                case InstructionKind.I32Load:
                case InstructionKind.I64Load:
                case InstructionKind.F32Load:
                case InstructionKind.F64Load:
                case InstructionKind.I32Load8S:
                case InstructionKind.I32Load8U:
                case InstructionKind.I32Load16S:
                case InstructionKind.I32Load16U:
                case InstructionKind.I64Load8S:
                case InstructionKind.I64Load8U:
                case InstructionKind.I64Load16S:
                case InstructionKind.I64Load16U:
                case InstructionKind.I64Load32S:
                case InstructionKind.I64Load32U:
                case InstructionKind.I32Store:
                case InstructionKind.I64Store:
                case InstructionKind.F32Store:
                case InstructionKind.F64Store:
                case InstructionKind.I32Store8S:
                case InstructionKind.I32Store16S:
                case InstructionKind.I64Store8S:
                case InstructionKind.I64Store16S:
                case InstructionKind.I64Store32S:
                    return OperandKind.MemArg;

                case InstructionKind.MemorySize:
                case InstructionKind.MemoryGrow:
                    return OperandKind.Zero;

                case InstructionKind.I32Const:
                    return OperandKind.ImmediateI32;
                case InstructionKind.I64Const:
                    return OperandKind.ImmediateI64;
                case InstructionKind.F32Const:
                    return OperandKind.ImmediateF32;
                case InstructionKind.F64Const:
                    return OperandKind.ImmediateF64;

                default: // anything after 0x45 does not have an operand
                    return OperandKind.None;
            }
        }
    }
}