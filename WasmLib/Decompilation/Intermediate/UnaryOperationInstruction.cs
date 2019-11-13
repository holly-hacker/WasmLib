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

        public UnaryOperationInstruction(in Instruction instruction)
        {
            (Type, Operation) = instruction.OpCode switch {
                OpCode.I32Clz => (ValueKind.I32, OperationKind.Clz),
                OpCode.I32Ctz => (ValueKind.I32, OperationKind.Ctz),
                OpCode.I32Popcnt => (ValueKind.I32, OperationKind.PopCnt),
                OpCode.I64Clz => (ValueKind.I64, OperationKind.Clz),
                OpCode.I64Ctz => (ValueKind.I64, OperationKind.Ctz),
                OpCode.I64Popcnt => (ValueKind.I64, OperationKind.PopCnt),

                OpCode.F32Abs => (ValueKind.F32, OperationKind.Abs),
                OpCode.F32Neg => (ValueKind.F32, OperationKind.Neg),
                OpCode.F32Ceil => (ValueKind.F32, OperationKind.Ceil),
                OpCode.F32Floor => (ValueKind.F32, OperationKind.Floor),
                OpCode.F32Trunc => (ValueKind.F32, OperationKind.Truncate),
                OpCode.F32Nearest => (ValueKind.F32, OperationKind.Nearest),
                OpCode.F32Sqrt => (ValueKind.F32, OperationKind.Sqrt),
                OpCode.F64Abs => (ValueKind.F64, OperationKind.Abs),
                OpCode.F64Neg => (ValueKind.F64, OperationKind.Neg),
                OpCode.F64Ceil => (ValueKind.F64, OperationKind.Ceil),
                OpCode.F64Floor => (ValueKind.F64, OperationKind.Floor),
                OpCode.F64Trunc => (ValueKind.F64, OperationKind.Truncate),
                OpCode.F64Nearest => (ValueKind.F64, OperationKind.Nearest),
                OpCode.F64Sqrt => (ValueKind.F64, OperationKind.Sqrt),

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