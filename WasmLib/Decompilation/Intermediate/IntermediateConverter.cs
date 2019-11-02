using System;
using System.Collections.Generic;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;

namespace WasmLib.Decompilation.Intermediate
{
    public class IntermediateConverter
    {
        private readonly FunctionBody function;

        public IntermediateConverter(FunctionBody function)
        {
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

        public static IntermediateInstruction? ConvertInstruction(Instruction instruction)
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
                case InstructionKind.I64And: // TODO: others
                    return new BinaryOperationInstruction(instruction);
                
                case InstructionKind.I32Eqz:
                    return new TestOperationInstruction(instruction);

                default:
                    throw new NotImplementedException($"Unimplemented instruction {instruction}");
            }
        }
    }
}