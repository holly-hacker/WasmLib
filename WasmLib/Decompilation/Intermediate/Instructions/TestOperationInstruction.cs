using System.ComponentModel;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class TestOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public OperationKind Operation { get; }
        public override bool IsPure => true;

        public TestOperationInstruction(in Instruction instruction)
        {
            (Type, Operation) = instruction.OpCode switch {
                OpCode.I32Eqz => (ValueKind.I32, OperationKind.Eqz),
                OpCode.I64Eqz => (ValueKind.I64, OperationKind.Eqz),
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
        }

        public override ValueKind[] PopTypes => new[] {Type};
        public override ValueKind[] PushTypes => new[] {ValueKind.I32};

        protected override string OperationStringFormat => EnumUtils.GetDescription(Operation);
        
        public override string ToString() => Operation.ToString();

        public enum OperationKind
        {
            [Description("{0} == 0")] Eqz,
        }
    }
}