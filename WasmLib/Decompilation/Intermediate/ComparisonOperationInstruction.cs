using System.ComponentModel;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class ComparisonOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public ComparisonKind Comparison { get; }
        public bool? IsSigned { get; }
        
        public ComparisonOperationInstruction(in Instruction instruction)
        {
            (Type, Comparison, IsSigned) = instruction.OpCode switch {
                InstructionKind.I32Eq => (ValueKind.I32, ComparisonKind.Equal, (bool?)null),
                InstructionKind.I32Ne => (ValueKind.I32, ComparisonKind.NotEqual, (bool?)null),
                InstructionKind.I32LtS => (ValueKind.I32, ComparisonKind.LessThan, true),
                InstructionKind.I32LtU => (ValueKind.I32, ComparisonKind.LessThan, false),
                InstructionKind.I32GtS => (ValueKind.I32, ComparisonKind.GreaterThan, true),
                InstructionKind.I32GtU => (ValueKind.I32, ComparisonKind.GreaterThan, false),
                InstructionKind.I32LeS => (ValueKind.I32, ComparisonKind.LessThanOrEqual, true),
                InstructionKind.I32LeU => (ValueKind.I32, ComparisonKind.LessThanOrEqual, false),
                InstructionKind.I32GeS => (ValueKind.I32, ComparisonKind.GreaterThanOrEqual, true),
                InstructionKind.I32GeU => (ValueKind.I32, ComparisonKind.GreaterThanOrEqual, false),

                InstructionKind.I64Eq => (ValueKind.I64, ComparisonKind.Equal, (bool?)null),
                InstructionKind.I64Ne => (ValueKind.I64, ComparisonKind.NotEqual, (bool?)null),
                InstructionKind.I64LtS => (ValueKind.I64, ComparisonKind.LessThan, true),
                InstructionKind.I64LtU => (ValueKind.I64, ComparisonKind.LessThan, false),
                InstructionKind.I64GtS => (ValueKind.I64, ComparisonKind.GreaterThan, true),
                InstructionKind.I64GtU => (ValueKind.I64, ComparisonKind.GreaterThan, false),
                InstructionKind.I64LeS => (ValueKind.I64, ComparisonKind.LessThanOrEqual, true),
                InstructionKind.I64LeU => (ValueKind.I64, ComparisonKind.LessThanOrEqual, false),
                InstructionKind.I64GeS => (ValueKind.I64, ComparisonKind.GreaterThanOrEqual, true),
                InstructionKind.I64GeU => (ValueKind.I64, ComparisonKind.GreaterThanOrEqual, false),

                InstructionKind.F32Eq => (ValueKind.F32, ComparisonKind.Equal, (bool?)null),
                InstructionKind.F32Ne => (ValueKind.F32, ComparisonKind.NotEqual, (bool?)null),
                InstructionKind.F32Lt => (ValueKind.F32, ComparisonKind.LessThan, (bool?)null),
                InstructionKind.F32Gt => (ValueKind.F32, ComparisonKind.GreaterThan, (bool?)null),
                InstructionKind.F32Le => (ValueKind.F32, ComparisonKind.LessThanOrEqual, (bool?)null),
                InstructionKind.F32Ge => (ValueKind.F32, ComparisonKind.GreaterThanOrEqual, (bool?)null),

                InstructionKind.F64Eq => (ValueKind.F64, ComparisonKind.Equal, (bool?)null),
                InstructionKind.F64Ne => (ValueKind.F64, ComparisonKind.NotEqual, (bool?)null),
                InstructionKind.F64Lt => (ValueKind.F64, ComparisonKind.LessThan, (bool?)null),
                InstructionKind.F64Gt => (ValueKind.F64, ComparisonKind.GreaterThan, (bool?)null),
                InstructionKind.F64Le => (ValueKind.F64, ComparisonKind.LessThanOrEqual, (bool?)null),
                InstructionKind.F64Ge => (ValueKind.F64, ComparisonKind.GreaterThanOrEqual, (bool?)null),
                
                _ => throw new WrongInstructionPassedException(instruction, nameof(ComparisonOperationInstruction)),
            };
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            var popped2 = context.Pop();
            Debug.Assert(popped2.Type == Type);
            var popped1 = context.Pop();
            Debug.Assert(popped1.Type == Type);

            var pushed = context.Push(ValueKind.I32);

            string comment = IsSigned switch {
                true => " // signed comparison",
                false => " // unsigned comparison",
                null => string.Empty,
            };
            
            context.WriteFull($"{pushed} = {popped1} {EnumUtils.GetDescription(Comparison)} {popped2}{comment}");
        }

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