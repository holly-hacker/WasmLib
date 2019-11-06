using System;
using System.Collections.Generic;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Decompilation.Intermediate
{
    public class IntermediateConverter
    {
        private readonly WasmFile wasmFile;
        private readonly FunctionBody function;

        public IntermediateConverter(WasmFile wasmFile, FunctionBody function)
        {
            this.wasmFile = wasmFile;
            this.function = function;
        }

        public List<IntermediateInstruction> Convert()
        {
            int start = 0;
            return ConvertBlock(ref start);
        }

        public List<IntermediateInstruction> ConvertBlock(ref int i, bool allowElse = false)
        {
            var list = new List<IntermediateInstruction>();

            for (; i < function.Instructions.Length; i++) {
                Instruction instruction = function.Instructions[i];
                switch (instruction.OpCode) {
                    case InstructionKind.End:
                    case InstructionKind.Else when allowElse:
                        return list;
                    case InstructionKind.Else:
                        throw new Exception($"Unexpected `{instruction}` instruction, else is not allowed in the current block");
                    case InstructionKind.Block:
                    case InstructionKind.Loop:
                    case InstructionKind.If:
                        i++;
                        List<IntermediateInstruction> list1 = ConvertBlock(ref i, instruction.OpCode == InstructionKind.If);
                        List<IntermediateInstruction>? list2 = null;

                        var instr = function.Instructions[i];
                        if (instr.OpCode == InstructionKind.Else) {
                            i++;
                            list2 = ConvertBlock(ref i);
                        }
                        else {
                            Debug.Assert(instr.OpCode == InstructionKind.End);
                        }
                        
                        list.Add(new ControlBlockInstruction(instruction, list1, list2));
                        break;
                    default:
                        IntermediateInstruction? intermediateInstruction = ConvertInstruction(instruction);

                        if (intermediateInstruction != null) {
                            list.Add(intermediateInstruction);
                        }

                        break;
                }
            }

            throw new IndexOutOfRangeException("Tried to read past end of function body, is the `end` instruction missing?");
        }

        public IntermediateInstruction? ConvertInstruction(Instruction instruction)
        {
            switch (instruction.OpCode) {
                case InstructionKind.Unreachable:
                    throw new NotImplementedException();
                case InstructionKind.Nop:
                    return null;
                
                case InstructionKind.Block:
                case InstructionKind.Loop:
                case InstructionKind.If:
                    throw new Exception($"Encountered control flow instruction '{instruction}' in wrong loop");
                case InstructionKind.Else:
                case InstructionKind.End:
                    throw new Exception($"Encountered unexpected control flow instruction '{instruction}'");
                
                case InstructionKind.Br:
                case InstructionKind.BrIf:
                case InstructionKind.BrTable:
                    return new BranchInstruction(instruction);
                
                case InstructionKind.Return:
                    return new ReturnInstruction();
                case InstructionKind.Call:
                case InstructionKind.CallIndirect:
                    return new CallInstruction(wasmFile, instruction);
                
                case InstructionKind.Drop:
                    return new DropInstruction();
                case InstructionKind.Select:
                    return new SelectInstruction();
                
                case InstructionKind.LocalGet:
                case InstructionKind.LocalSet:
                case InstructionKind.LocalTee:
                case InstructionKind.GlobalGet:
                case InstructionKind.GlobalSet:
                    return new VariableInstruction(instruction);

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
                case InstructionKind.I32Store8:
                case InstructionKind.I32Store16:
                case InstructionKind.I64Store8:
                case InstructionKind.I64Store16:
                case InstructionKind.I64Store32:
                    return new MemoryInstruction(instruction);
                
                case InstructionKind.MemoryGrow:
                case InstructionKind.MemorySize:
                    return new MemorySizeInstruction(instruction);
                
                case InstructionKind.I32Const:
                case InstructionKind.I64Const:
                case InstructionKind.F32Const:
                case InstructionKind.F64Const:
                    return new ConstInstruction(instruction);
                
                case InstructionKind.I32Add:
                case InstructionKind.I64Add:
                case InstructionKind.F32Add:
                case InstructionKind.F64Add:
                case InstructionKind.I32Sub:
                case InstructionKind.I64Sub:
                case InstructionKind.F32Sub:
                case InstructionKind.F64Sub:
                case InstructionKind.I32Mul:
                case InstructionKind.I64Mul:
                case InstructionKind.F32Mul:
                case InstructionKind.F64Mul:
                    
                case InstructionKind.I32And:
                case InstructionKind.I64And:
                case InstructionKind.I32Or:
                case InstructionKind.I64Or:
                case InstructionKind.I32Xor:
                case InstructionKind.I64Xor:
                case InstructionKind.I32Shl:
                case InstructionKind.I64Shl:
                case InstructionKind.I32ShrS:
                case InstructionKind.I64ShrS:
                case InstructionKind.I32ShrU:
                case InstructionKind.I64ShrU:
                case InstructionKind.I32Rotl:
                case InstructionKind.I64Rotl:
                case InstructionKind.I32Rotr:
                case InstructionKind.I64Rotr:
                    return new BinaryOperationInstruction(instruction);


                case InstructionKind.I32Eq:
                case InstructionKind.I32Ne:
                case InstructionKind.I32LtS:
                case InstructionKind.I32LtU:
                case InstructionKind.I32GtS:
                case InstructionKind.I32GtU:
                case InstructionKind.I32LeS:
                case InstructionKind.I32LeU:
                case InstructionKind.I32GeS:
                case InstructionKind.I32GeU:

                case InstructionKind.I64Eq:
                case InstructionKind.I64Ne:
                case InstructionKind.I64LtS:
                case InstructionKind.I64LtU:
                case InstructionKind.I64GtS:
                case InstructionKind.I64GtU:
                case InstructionKind.I64LeS:
                case InstructionKind.I64LeU:
                case InstructionKind.I64GeS:
                case InstructionKind.I64GeU:

                case InstructionKind.F32Eq:
                case InstructionKind.F32Ne:
                case InstructionKind.F32Lt:
                case InstructionKind.F32Gt:
                case InstructionKind.F32Le:
                case InstructionKind.F32Ge:

                case InstructionKind.F64Eq:
                case InstructionKind.F64Ne:
                case InstructionKind.F64Lt:
                case InstructionKind.F64Gt:
                case InstructionKind.F64Le:
                case InstructionKind.F64Ge:
                return new ComparisonOperationInstruction(instruction);
                
                case InstructionKind.I32Eqz:
                case InstructionKind.I64Eqz:
                    return new TestOperationInstruction(instruction);

                case InstructionKind.I32WrapI64:
                case InstructionKind.I32TruncF32S:
                case InstructionKind.I32TruncF32U:
                case InstructionKind.I32TruncF64S:
                case InstructionKind.I32TruncF64U:
                case InstructionKind.I64ExtendI32S:
                case InstructionKind.I64ExtendI32U:
                case InstructionKind.I64TruncF32S:
                case InstructionKind.I64TruncF32U:
                case InstructionKind.I64TruncF64S:
                case InstructionKind.I64TruncF64U:
                case InstructionKind.F32ConvertI32S:
                case InstructionKind.F32ConvertI32U:
                case InstructionKind.F32ConvertI64S:
                case InstructionKind.F32ConvertI64U:
                case InstructionKind.F32DemoteF64:
                case InstructionKind.F64ConvertI32S:
                case InstructionKind.F64ConvertI32U:
                case InstructionKind.F64ConvertI64S:
                case InstructionKind.F64ConvertI64U:
                case InstructionKind.F64PromoteF32:
                case InstructionKind.I32ReinterpretF32:
                case InstructionKind.I64ReinterpretF64:
                case InstructionKind.F32ReinterpretI32:
                case InstructionKind.F64ReinterpretI64:
                    return new ConversionInstruction(instruction);

                default:
                    throw new NotImplementedException($"Unimplemented instruction {instruction}");
            }
        }
    }
}