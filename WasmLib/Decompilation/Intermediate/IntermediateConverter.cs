using System;
using System.Collections.Generic;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Decompilation.Intermediate
{
    public class IntermediateConverter
    {
        private readonly WasmModule wasmModule;
        private readonly FunctionBody function;
        private readonly FunctionSignature signature;

        public IntermediateConverter(WasmModule wasmModule, FunctionBody function, FunctionSignature signature)
        {
            this.wasmModule = wasmModule;
            this.function = function;
            this.signature = signature;
        }

        public List<IntermediateInstruction> Convert()
        {
            int start = 0;
            var block = ConvertBlock(ref start);
            
            // add implicit return if required
            if (signature.ReturnParameter.Length != 0) {
                block.Add(new ImplicitReturnInstruction(signature));
            }

            return block;
        }

        private List<IntermediateInstruction> ConvertBlock(ref int i, bool allowElse = false)
        {
            var list = new List<IntermediateInstruction>();

            for (; i < function.Instructions.Length; i++) {
                Instruction instruction = function.Instructions[i];
                switch (instruction.OpCode) {
                    case OpCode.End:
                    case OpCode.Else when allowElse:
                        return list;
                    case OpCode.Else:
                        throw new Exception($"Unexpected `{instruction}` instruction, else is not allowed in the current block");
                    case OpCode.Block:
                    case OpCode.Loop:
                    case OpCode.If:
                        i++;
                        List<IntermediateInstruction> list1 = ConvertBlock(ref i, instruction.OpCode == OpCode.If);
                        List<IntermediateInstruction>? list2 = null;

                        var instr = function.Instructions[i];
                        if (instr.OpCode == OpCode.Else) {
                            i++;
                            list2 = ConvertBlock(ref i);
                        }
                        else {
                            Debug.Assert(instr.OpCode == OpCode.End);
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
                case OpCode.Unreachable:
                    return new UnreachableInstruction();
                case OpCode.Nop:
                    return null;
                
                case OpCode.Block:
                case OpCode.Loop:
                case OpCode.If:
                    throw new Exception($"Encountered control flow instruction '{instruction}' in wrong loop");
                case OpCode.Else:
                case OpCode.End:
                    throw new Exception($"Encountered unexpected control flow instruction '{instruction}'");
                
                case OpCode.Br:
                case OpCode.BrIf:
                case OpCode.BrTable:
                    return new BranchInstruction(instruction);
                
                case OpCode.Return:
                    return new ReturnInstruction(signature);
                case OpCode.Call:
                case OpCode.CallIndirect:
                    return new CallInstruction(instruction, wasmModule);
                
                case OpCode.Drop:
                    return new DropInstruction();
                case OpCode.Select:
                    return new SelectInstruction();
                
                case OpCode.LocalGet:
                case OpCode.LocalSet:
                case OpCode.LocalTee:
                case OpCode.GlobalGet:
                case OpCode.GlobalSet:
                    return new VariableInstruction(instruction, wasmModule, function, signature);

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
                    return new MemoryInstruction(instruction);
                
                case OpCode.MemoryGrow:
                case OpCode.MemorySize:
                    return new MemorySizeInstruction(instruction);
                
                case OpCode.I32Const:
                case OpCode.I64Const:
                case OpCode.F32Const:
                case OpCode.F64Const:
                    return new ConstInstruction(instruction);
                
                case OpCode.I32Clz:
                case OpCode.I32Ctz:
                case OpCode.I32Popcnt:
                case OpCode.I64Clz:
                case OpCode.I64Ctz:
                case OpCode.I64Popcnt:
                    
                case OpCode.F32Abs:
                case OpCode.F32Neg:
                case OpCode.F32Ceil:
                case OpCode.F32Floor:
                case OpCode.F32Trunc:
                case OpCode.F32Nearest:
                case OpCode.F32Sqrt:
                case OpCode.F64Abs:
                case OpCode.F64Neg:
                case OpCode.F64Ceil:
                case OpCode.F64Floor:
                case OpCode.F64Trunc:
                case OpCode.F64Nearest:
                case OpCode.F64Sqrt:
                    return new UnaryOperationInstruction(instruction);
                
                case OpCode.I32Add:
                case OpCode.I64Add:
                case OpCode.F32Add:
                case OpCode.F64Add:
                case OpCode.I32Sub:
                case OpCode.I64Sub:
                case OpCode.F32Sub:
                case OpCode.F64Sub:
                case OpCode.I32Mul:
                case OpCode.I64Mul:
                case OpCode.F32Mul:
                case OpCode.F64Mul:
                    
                case OpCode.I32DivS:
                case OpCode.I32DivU:
                case OpCode.I32RemS:
                case OpCode.I32RemU:
                case OpCode.I64DivS:
                case OpCode.I64DivU:
                case OpCode.I64RemS:
                case OpCode.I64RemU:
                case OpCode.I32And:
                case OpCode.I64And:
                case OpCode.I32Or:
                case OpCode.I64Or:
                case OpCode.I32Xor:
                case OpCode.I64Xor:
                case OpCode.I32Shl:
                case OpCode.I64Shl:
                case OpCode.I32ShrS:
                case OpCode.I64ShrS:
                case OpCode.I32ShrU:
                case OpCode.I64ShrU:
                case OpCode.I32Rotl:
                case OpCode.I64Rotl:
                case OpCode.I32Rotr:
                case OpCode.I64Rotr:
                    
                case OpCode.F32Div:
                case OpCode.F32Min:
                case OpCode.F32Max:
                case OpCode.F32Copysign:
                case OpCode.F64Div:
                case OpCode.F64Min:
                case OpCode.F64Max:
                case OpCode.F64Copysign:
                    return new BinaryOperationInstruction(instruction);


                case OpCode.I32Eq:
                case OpCode.I32Ne:
                case OpCode.I32LtS:
                case OpCode.I32LtU:
                case OpCode.I32GtS:
                case OpCode.I32GtU:
                case OpCode.I32LeS:
                case OpCode.I32LeU:
                case OpCode.I32GeS:
                case OpCode.I32GeU:

                case OpCode.I64Eq:
                case OpCode.I64Ne:
                case OpCode.I64LtS:
                case OpCode.I64LtU:
                case OpCode.I64GtS:
                case OpCode.I64GtU:
                case OpCode.I64LeS:
                case OpCode.I64LeU:
                case OpCode.I64GeS:
                case OpCode.I64GeU:

                case OpCode.F32Eq:
                case OpCode.F32Ne:
                case OpCode.F32Lt:
                case OpCode.F32Gt:
                case OpCode.F32Le:
                case OpCode.F32Ge:

                case OpCode.F64Eq:
                case OpCode.F64Ne:
                case OpCode.F64Lt:
                case OpCode.F64Gt:
                case OpCode.F64Le:
                case OpCode.F64Ge:
                return new ComparisonOperationInstruction(instruction);
                
                case OpCode.I32Eqz:
                case OpCode.I64Eqz:
                    return new TestOperationInstruction(instruction);

                case OpCode.I32WrapI64:
                case OpCode.I32TruncF32S:
                case OpCode.I32TruncF32U:
                case OpCode.I32TruncF64S:
                case OpCode.I32TruncF64U:
                case OpCode.I64ExtendI32S:
                case OpCode.I64ExtendI32U:
                case OpCode.I64TruncF32S:
                case OpCode.I64TruncF32U:
                case OpCode.I64TruncF64S:
                case OpCode.I64TruncF64U:
                case OpCode.F32ConvertI32S:
                case OpCode.F32ConvertI32U:
                case OpCode.F32ConvertI64S:
                case OpCode.F32ConvertI64U:
                case OpCode.F32DemoteF64:
                case OpCode.F64ConvertI32S:
                case OpCode.F64ConvertI32U:
                case OpCode.F64ConvertI64S:
                case OpCode.F64ConvertI64U:
                case OpCode.F64PromoteF32:
                case OpCode.I32ReinterpretF32:
                case OpCode.I64ReinterpretF64:
                case OpCode.F32ReinterpretI32:
                case OpCode.F64ReinterpretI64:
                    return new ConversionOperatorInstruction(instruction);

                default:
                    throw new NotImplementedException($"Unimplemented instruction {instruction}");
            }
        }
    }
}