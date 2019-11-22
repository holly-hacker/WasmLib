using System.ComponentModel;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class ComparisonOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public ComparisonKind Comparison { get; }
        public bool? IsSigned { get; }
        public override bool IsPure => true;
        
        public ComparisonOperationInstruction(in Instruction instruction)
        {
            (Type, Comparison, IsSigned) = instruction.OpCode switch {
                OpCode.I32Eq => (ValueKind.I32, ComparisonKind.Equal, (bool?)null),
                OpCode.I32Ne => (ValueKind.I32, ComparisonKind.NotEqual, (bool?)null),
                OpCode.I32LtS => (ValueKind.I32, ComparisonKind.LessThan, true),
                OpCode.I32LtU => (ValueKind.I32, ComparisonKind.LessThan, false),
                OpCode.I32GtS => (ValueKind.I32, ComparisonKind.GreaterThan, true),
                OpCode.I32GtU => (ValueKind.I32, ComparisonKind.GreaterThan, false),
                OpCode.I32LeS => (ValueKind.I32, ComparisonKind.LessThanOrEqual, true),
                OpCode.I32LeU => (ValueKind.I32, ComparisonKind.LessThanOrEqual, false),
                OpCode.I32GeS => (ValueKind.I32, ComparisonKind.GreaterThanOrEqual, true),
                OpCode.I32GeU => (ValueKind.I32, ComparisonKind.GreaterThanOrEqual, false),

                OpCode.I64Eq => (ValueKind.I64, ComparisonKind.Equal, (bool?)null),
                OpCode.I64Ne => (ValueKind.I64, ComparisonKind.NotEqual, (bool?)null),
                OpCode.I64LtS => (ValueKind.I64, ComparisonKind.LessThan, true),
                OpCode.I64LtU => (ValueKind.I64, ComparisonKind.LessThan, false),
                OpCode.I64GtS => (ValueKind.I64, ComparisonKind.GreaterThan, true),
                OpCode.I64GtU => (ValueKind.I64, ComparisonKind.GreaterThan, false),
                OpCode.I64LeS => (ValueKind.I64, ComparisonKind.LessThanOrEqual, true),
                OpCode.I64LeU => (ValueKind.I64, ComparisonKind.LessThanOrEqual, false),
                OpCode.I64GeS => (ValueKind.I64, ComparisonKind.GreaterThanOrEqual, true),
                OpCode.I64GeU => (ValueKind.I64, ComparisonKind.GreaterThanOrEqual, false),

                OpCode.F32Eq => (ValueKind.F32, ComparisonKind.Equal, (bool?)null),
                OpCode.F32Ne => (ValueKind.F32, ComparisonKind.NotEqual, (bool?)null),
                OpCode.F32Lt => (ValueKind.F32, ComparisonKind.LessThan, (bool?)null),
                OpCode.F32Gt => (ValueKind.F32, ComparisonKind.GreaterThan, (bool?)null),
                OpCode.F32Le => (ValueKind.F32, ComparisonKind.LessThanOrEqual, (bool?)null),
                OpCode.F32Ge => (ValueKind.F32, ComparisonKind.GreaterThanOrEqual, (bool?)null),

                OpCode.F64Eq => (ValueKind.F64, ComparisonKind.Equal, (bool?)null),
                OpCode.F64Ne => (ValueKind.F64, ComparisonKind.NotEqual, (bool?)null),
                OpCode.F64Lt => (ValueKind.F64, ComparisonKind.LessThan, (bool?)null),
                OpCode.F64Gt => (ValueKind.F64, ComparisonKind.GreaterThan, (bool?)null),
                OpCode.F64Le => (ValueKind.F64, ComparisonKind.LessThanOrEqual, (bool?)null),
                OpCode.F64Ge => (ValueKind.F64, ComparisonKind.GreaterThanOrEqual, (bool?)null),
                
                _ => throw new WrongInstructionPassedException(instruction, nameof(ComparisonOperationInstruction)),
            };
        }

        public override ValueKind[] PopTypes => new[] {Type, Type};
        public override ValueKind[] PushTypes => new[] {ValueKind.I32};

        public override string OperationStringFormat => $@"{{1}} {EnumUtils.GetDescription(Comparison)} {{0}}";

        public override string? Comment => IsSigned switch {
            true => "signed comparison",
            false => "unsigned comparison",
            null => null,
        };

        public override string ToString() => Comparison.ToString();

        public enum ComparisonKind
        {
            [Description("==")] Equal,
            [Description("!=")] NotEqual,
            [Description("<")] LessThan,
            [Description(">")] GreaterThan,
            [Description("<=")] LessThanOrEqual,
            [Description(">=")] GreaterThanOrEqual,
        }
    }
}