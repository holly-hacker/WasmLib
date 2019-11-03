using System.ComponentModel;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class BinaryOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public OperationKind Operation { get; }
        
        public BinaryOperationInstruction(Instruction instruction)
        {
            (Type, Operation) = instruction.OpCode switch {
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
                // TODO: div, rem
                InstructionKind.I32And => (ValueKind.I32, OperationKind.And),
                InstructionKind.I64And => (ValueKind.I64, OperationKind.And),
                InstructionKind.I32Or => (ValueKind.I32, OperationKind.Or),
                InstructionKind.I64Or => (ValueKind.I64, OperationKind.Or),
                InstructionKind.I32Xor => (ValueKind.I32, OperationKind.Xor),
                InstructionKind.I64Xor => (ValueKind.I64, OperationKind.Xor),
                InstructionKind.I32Shl => (ValueKind.I32, OperationKind.Shl),
                InstructionKind.I64Shl => (ValueKind.I64, OperationKind.Shl),
                InstructionKind.I32ShrS => (ValueKind.I32, OperationKind.ShrS),
                InstructionKind.I64ShrS => (ValueKind.I64, OperationKind.ShrS),
                InstructionKind.I32ShrU => (ValueKind.I32, OperationKind.ShrU),
                InstructionKind.I64ShrU => (ValueKind.I64, OperationKind.ShrU),
                InstructionKind.I32Rotl => (ValueKind.I32, OperationKind.RotL),
                InstructionKind.I64Rotl => (ValueKind.I64, OperationKind.RotL),
                InstructionKind.I32Rotr => (ValueKind.I32, OperationKind.RotR),
                InstructionKind.I64Rotr => (ValueKind.I64, OperationKind.RotR),
                // float
                // TODO: add missing
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
            
            context.WriteFull($"{pushed} = {popped1} {EnumUtils.GetDescription(Operation)} {popped2}");
        }

        public enum OperationKind
        {
            // both
            [Description("+")] Add,
            [Description("*")] Sub,
            [Description("*")] Mul,
            // int
            // TODO: div, rem
            [Description("&")] And,
            [Description("|")] Or,
            [Description("^")] Xor,
            [Description("<<")] Shl,
            [Description(">>")] ShrS,
            [Description(">>>")] ShrU,
            [Description("ROTL")] RotL,
            [Description("ROTR")] RotR,
            // float
        }
    }
}