using System.ComponentModel;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class BinaryOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public OperationKind Operation { get; }
        public bool? IsSigned { get; }

        public override ValueKind[] PopTypes => new[] {Type, Type};
        public override ValueKind[] PushTypes => new[] {Type};

        public BinaryOperationInstruction(in Instruction instruction)
        {
            (Type, Operation, IsSigned) = instruction.OpCode switch {
                // both
                OpCode.I32Add => (ValueKind.I32, OperationKind.Add, (bool?)null),
                OpCode.I64Add => (ValueKind.I64, OperationKind.Add, (bool?)null),
                OpCode.F32Add => (ValueKind.F32, OperationKind.Add, (bool?)null),
                OpCode.F64Add => (ValueKind.F64, OperationKind.Add, (bool?)null),
                OpCode.I32Sub => (ValueKind.I32, OperationKind.Sub, (bool?)null),
                OpCode.I64Sub => (ValueKind.I64, OperationKind.Sub, (bool?)null),
                OpCode.F32Sub => (ValueKind.F32, OperationKind.Sub, (bool?)null),
                OpCode.F64Sub => (ValueKind.F64, OperationKind.Sub, (bool?)null),
                OpCode.I32Mul => (ValueKind.I32, OperationKind.Mul, (bool?)null),
                OpCode.I64Mul => (ValueKind.I64, OperationKind.Mul, (bool?)null),
                OpCode.F32Mul => (ValueKind.F32, OperationKind.Mul, (bool?)null),
                OpCode.F64Mul => (ValueKind.F64, OperationKind.Mul, (bool?)null),
                // int
                OpCode.I32DivS => (ValueKind.I32, OperationKind.Div, true),
                OpCode.I32DivU => (ValueKind.I32, OperationKind.Div, false),
                OpCode.I32RemS => (ValueKind.I32, OperationKind.Rem, true),
                OpCode.I32RemU => (ValueKind.I32, OperationKind.Rem, false),
                OpCode.I64DivS => (ValueKind.I64, OperationKind.Div, true),
                OpCode.I64DivU => (ValueKind.I64, OperationKind.Div, false),
                OpCode.I64RemS => (ValueKind.I64, OperationKind.Rem, true),
                OpCode.I64RemU => (ValueKind.I64, OperationKind.Rem, false),
                OpCode.I32And => (ValueKind.I32, OperationKind.And, (bool?)null),
                OpCode.I64And => (ValueKind.I64, OperationKind.And, (bool?)null),
                OpCode.I32Or => (ValueKind.I32, OperationKind.Or, (bool?)null),
                OpCode.I64Or => (ValueKind.I64, OperationKind.Or, (bool?)null),
                OpCode.I32Xor => (ValueKind.I32, OperationKind.Xor, (bool?)null),
                OpCode.I64Xor => (ValueKind.I64, OperationKind.Xor, (bool?)null),
                OpCode.I32Shl => (ValueKind.I32, OperationKind.Shl, (bool?)null),
                OpCode.I64Shl => (ValueKind.I64, OperationKind.Shl, (bool?)null),
                OpCode.I32ShrS => (ValueKind.I32, OperationKind.ShrS, (bool?)null),
                OpCode.I64ShrS => (ValueKind.I64, OperationKind.ShrS, (bool?)null),
                OpCode.I32ShrU => (ValueKind.I32, OperationKind.ShrU, (bool?)null),
                OpCode.I64ShrU => (ValueKind.I64, OperationKind.ShrU, (bool?)null),
                OpCode.I32Rotl => (ValueKind.I32, OperationKind.RotL, (bool?)null),
                OpCode.I64Rotl => (ValueKind.I64, OperationKind.RotL, (bool?)null),
                OpCode.I32Rotr => (ValueKind.I32, OperationKind.RotR, (bool?)null),
                OpCode.I64Rotr => (ValueKind.I64, OperationKind.RotR, (bool?)null),
                // float
                OpCode.F32Div => (ValueKind.F32, OperationKind.Div, (bool?)null),
                OpCode.F32Min => (ValueKind.F32, OperationKind.Min, (bool?)null),
                OpCode.F32Max => (ValueKind.F32, OperationKind.Max, (bool?)null),
                OpCode.F32Copysign => (ValueKind.F32, OperationKind.CopySign, (bool?)null),
                OpCode.F64Div => (ValueKind.F64, OperationKind.Div, (bool?)null),
                OpCode.F64Min => (ValueKind.F64, OperationKind.Min, (bool?)null),
                OpCode.F64Max => (ValueKind.F64, OperationKind.Max, (bool?)null),
                OpCode.F64Copysign => (ValueKind.F64, OperationKind.CopySign, (bool?)null),
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
        }

        public override string OperationStringFormat => Operation switch {
            OperationKind.Min => "min({1}, {0})",
            OperationKind.Max => "max({1}, {0})",
            OperationKind.CopySign => "copysign({1}, {0})",
            _ => $"{{1}} {EnumUtils.GetDescription(Operation)} {{0}}"
        };

        public override string? Comment => IsSigned.HasValue ? $"{(IsSigned.Value ? "signed" : "unsigned")} operation" : null;

        public override string ToString() => Operation.ToString();

        public enum OperationKind
        {
            // both
            [Description("+")] Add,
            [Description("*")] Sub,
            [Description("*")] Mul,
            [Description("/")] Div,
            // int
            [Description("%")] Rem,
            [Description("&")] And,
            [Description("|")] Or,
            [Description("^")] Xor,
            [Description("<<")] Shl,
            [Description(">>")] ShrS,
            [Description(">>>")] ShrU,
            [Description("ROTL")] RotL,
            [Description("ROTR")] RotR,
            // float
            Min,
            Max,
            CopySign,
        }
    }
}