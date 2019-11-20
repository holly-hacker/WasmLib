using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class ConversionOperatorInstruction : IntermediateInstruction
    {
        public OperationKind Operation { get; }
        /// <remarks> t2 in <code>t2.cvtop_t1_sx</code> </remarks>
        public ValueKind TargetType { get; }
        /// <remarks> t1 in <code>t2.cvtop_t1_sx</code> </remarks>
        public ValueKind SourceType { get; }
        public bool? IsSigned { get; }
        public override bool IsPure => true;

        public ConversionOperatorInstruction(in Instruction instruction)
        {
            (Operation, TargetType, SourceType, IsSigned) = instruction.OpCode switch {
                OpCode.I32WrapI64 => (OperationKind.Wrap, ValueKind.I32, ValueKind.I64, (bool?)null),
                OpCode.I32TruncF32S => (OperationKind.Truncate, ValueKind.I32, ValueKind.F32, true),
                OpCode.I32TruncF32U => (OperationKind.Truncate, ValueKind.I32, ValueKind.F32, false),
                OpCode.I32TruncF64S => (OperationKind.Truncate, ValueKind.I32, ValueKind.F64, true),
                OpCode.I32TruncF64U => (OperationKind.Truncate, ValueKind.I32, ValueKind.F64, false),
                OpCode.I64ExtendI32S => (OperationKind.Extend, ValueKind.I64, ValueKind.I32, true),
                OpCode.I64ExtendI32U => (OperationKind.Extend, ValueKind.I64, ValueKind.I32, false),
                OpCode.I64TruncF32S => (OperationKind.Truncate, ValueKind.I64, ValueKind.F32, true),
                OpCode.I64TruncF32U => (OperationKind.Truncate, ValueKind.I64, ValueKind.F32, false),
                OpCode.I64TruncF64S => (OperationKind.Truncate, ValueKind.I64, ValueKind.F64, true),
                OpCode.I64TruncF64U => (OperationKind.Truncate, ValueKind.I64, ValueKind.F64, false),
                OpCode.F32ConvertI32S => (OperationKind.Convert, ValueKind.F32, ValueKind.I32, true),
                OpCode.F32ConvertI32U => (OperationKind.Convert, ValueKind.F32, ValueKind.I32, false),
                OpCode.F32ConvertI64S => (OperationKind.Convert, ValueKind.F32, ValueKind.I64, true),
                OpCode.F32ConvertI64U => (OperationKind.Convert, ValueKind.F32, ValueKind.I64, false),
                OpCode.F32DemoteF64 => (OperationKind.Demote, ValueKind.F32, ValueKind.F64, (bool?)null),
                OpCode.F64ConvertI32S => (OperationKind.Convert, ValueKind.F64, ValueKind.I32, true),
                OpCode.F64ConvertI32U => (OperationKind.Convert, ValueKind.F64, ValueKind.I32, false),
                OpCode.F64ConvertI64S => (OperationKind.Convert, ValueKind.F64, ValueKind.I64, true),
                OpCode.F64ConvertI64U => (OperationKind.Convert, ValueKind.F64, ValueKind.I64, false),
                OpCode.F64PromoteF32 => (OperationKind.Promote, ValueKind.F64, ValueKind.F32, (bool?)null),
                OpCode.I32ReinterpretF32 => (OperationKind.Reinterpret, ValueKind.I32, ValueKind.F32, (bool?)null),
                OpCode.I64ReinterpretF64 => (OperationKind.Reinterpret, ValueKind.I64, ValueKind.F64, (bool?)null),
                OpCode.F32ReinterpretI32 => (OperationKind.Reinterpret, ValueKind.F32, ValueKind.I32, (bool?)null),
                OpCode.F64ReinterpretI64 => (OperationKind.Reinterpret, ValueKind.F64, ValueKind.I64, (bool?)null),
                _ => throw new WrongInstructionPassedException(instruction, nameof(ConversionOperatorInstruction)),
            };
        }

        public override ValueKind[] PopTypes => new[] {SourceType};
        public override ValueKind[] PushTypes => new[] {TargetType};

        protected override string OperationStringFormat {
            get {
                string targetString = EnumUtils.GetDescription(TargetType);

                if (Operation == OperationKind.Reinterpret) {
                    return $"{{0}} = *({targetString}*)&{{1}}";
                }

                return !IsSigned.HasValue
                    ? $"{{0}} = ({targetString}){{1}}"
                    : $"{{0}} = ({targetString}){{1}} // signed: {IsSigned}";
            }
        }

        public enum OperationKind
        {
            Wrap,
            Extend,
            Truncate,
            Convert,
            Demote,
            Promote,
            Reinterpret,
        }
    }
}