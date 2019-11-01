using System;
using System.Collections.Generic;
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

        public IEnumerable<IntermediateInstruction> Convert()
        {
            for (int i = 0; i < function.Body.Length - 1; i++) { // 1 less for `end` instruction
                IntermediateInstruction? instruction = ReadInstruction(ref i);

                if (instruction != null) {
                    yield return instruction;
                }
            }
        }

        private IntermediateInstruction? ReadInstruction(ref int i)
        {
            var instruction = function.Body[i];
            
            switch (instruction.Opcode) {
                case InstructionKind.Unreachable:
                    throw new NotImplementedException();
                case InstructionKind.Nop:
                    return null;
                
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

                default:
                    throw new NotImplementedException($"Unimplemented instruction {function.Body[i]}");
            }
        }
    }
}