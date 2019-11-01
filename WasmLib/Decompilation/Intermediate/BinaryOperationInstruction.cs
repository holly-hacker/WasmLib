using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class BinaryOperationInstruction : IntermediateInstruction
    {
        private static readonly Dictionary<OperationKind, string> OperationStringCache = new Dictionary<OperationKind, string>();
        
        public ValueKind Type { get; }
        public OperationKind Operation { get; }
        
        public BinaryOperationInstruction(Instruction instruction)
        {
            (Type, Operation) = instruction.Opcode switch {
                // both
                InstructionKind.I32Add => (ValueKind.I32, OperationKind.Add),
                InstructionKind.I64Add => (ValueKind.I64, OperationKind.Add),
                InstructionKind.F32Add => (ValueKind.F32, OperationKind.Add),
                InstructionKind.F64Add => (ValueKind.F64, OperationKind.Add),
                InstructionKind.I32Sub => (ValueKind.I32, OperationKind.Sub),
                InstructionKind.I64Sub => (ValueKind.I64, OperationKind.Sub),
                InstructionKind.F32Sub => (ValueKind.F32, OperationKind.Sub),
                InstructionKind.F64Sub => (ValueKind.F64, OperationKind.Sub),
                InstructionKind.I32Mul => (ValueKind.I32, OperationKind.Mul),
                InstructionKind.I64Mul => (ValueKind.I64, OperationKind.Mul),
                InstructionKind.F32Mul => (ValueKind.F32, OperationKind.Mul),
                InstructionKind.F64Mul => (ValueKind.F64, OperationKind.Mul),
                // int
                InstructionKind.I32And => (ValueKind.I32, OperationKind.And),
                InstructionKind.I64And => (ValueKind.I64, OperationKind.And),
                // float
                // TODO: add missing ones
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
        }

        public override void Handle(ref IntermediateContext context)
        {
            var popped2 = context.Pop();
            Debug.Assert(popped2.Type == Type, $"Popped operand 2 of type {popped2.Type} in {Type}{Operation} instruction");
            var popped1 = context.Pop();
            Debug.Assert(popped1.Type == Type, $"Popped operand 1 of type {popped1.Type} in {Type}{Operation} instruction");

            var pushed = context.Push(Type);
            
            context.WriteFull($"{pushed} = {popped1} {GetOperationString(Operation)} {popped2}");
        }

        private static string GetOperationString(OperationKind op) // TODO: this is duplicated in Instruction_Utils.cs, move it to a utility method
        {
            if (OperationStringCache.TryGetValue(op, out string? s)) {
                return s!;
            }
            
            var name = Enum.GetName(typeof(OperationKind), op);

            if (name is null) {
                return op.ToString();
            }

            var description = typeof(OperationKind)
                .GetField(name)?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description;

            if (description is null) {
                return op.ToString();
            }

            return OperationStringCache[op] = description;
        }

        public enum OperationKind
        {
            // both
            [Description("+")] Add,
            [Description("*")] Sub,
            [Description("*")] Mul,
            // int
            [Description("&")] And,
            // float
        }
    }
}