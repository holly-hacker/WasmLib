using System.ComponentModel;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class UnaryOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public OperationKind Operation { get; }

        public UnaryOperationInstruction(Instruction instruction)
        {
            (Type, Operation) = instruction.OpCode switch {
                InstructionKind.I32Clz => (ValueKind.I32, OperationKind.Clz),
                InstructionKind.I32Ctz => (ValueKind.I32, OperationKind.Ctz),
                InstructionKind.I32Popcnt => (ValueKind.I32, OperationKind.PopCnt),
                InstructionKind.I64Clz => (ValueKind.I64, OperationKind.Clz),
                InstructionKind.I64Ctz => (ValueKind.I64, OperationKind.Ctz),
                InstructionKind.I64Popcnt => (ValueKind.I64, OperationKind.PopCnt),

                InstructionKind.F32Abs => (ValueKind.F32, OperationKind.Abs),
                InstructionKind.F32Neg => (ValueKind.F32, OperationKind.Neg),
                InstructionKind.F32Ceil => (ValueKind.F32, OperationKind.Ceil),
                InstructionKind.F32Floor => (ValueKind.F32, OperationKind.Floor),
                InstructionKind.F32Trunc => (ValueKind.F32, Trunc: OperationKind.Truncate),
                InstructionKind.F32Nearest => (ValueKind.F32, OperationKind.Nearest),
                InstructionKind.F32Sqrt => (ValueKind.F32, OperationKind.Sqrt),
                InstructionKind.F64Abs => (ValueKind.F64, OperationKind.Abs),
                InstructionKind.F64Neg => (ValueKind.F64, OperationKind.Neg),
                InstructionKind.F64Ceil => (ValueKind.F64, OperationKind.Ceil),
                InstructionKind.F64Floor => (ValueKind.F64, OperationKind.Floor),
                InstructionKind.F64Trunc => (ValueKind.F64, Trunc: OperationKind.Truncate),
                InstructionKind.F64Nearest => (ValueKind.F64, OperationKind.Nearest),
                InstructionKind.F64Sqrt => (ValueKind.F64, OperationKind.Sqrt),

                _ => throw new WrongInstructionPassedException(instruction, nameof(UnaryOperationInstruction)),
            };
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            var popped = context.Pop();
            Debug.Assert(popped.Type == Type);

            var pushed = context.Push(Type);

            context.WriteFull(Operation == OperationKind.Neg
                ? $"{pushed} = {EnumUtils.GetDescription(Operation)}{popped}"
                : $"{pushed} = {EnumUtils.GetDescription(Operation)}({popped})");
        }

        public enum OperationKind
        {
            [Description("clz")] Clz,
            [Description("ctz")] Ctz,
            [Description("popcnt")] PopCnt,

            [Description("abs")] Abs,
            [Description("-")] Neg,
            [Description("sqrt")] Sqrt,
            [Description("ceil")] Ceil,
            [Description("floor")] Floor,
            [Description("chop")] Truncate,
            [Description("roundeven")] Nearest,
        }
    }
}