using System.ComponentModel;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class TestOperationInstruction : IntermediateInstruction
    {
        public ValueKind Type { get; }
        public OperationKind Operation { get; }
        
        public TestOperationInstruction(Instruction instruction)
        {
            (Type, Operation) = instruction.OpCode switch {
                OpCode.I32Eqz => (ValueKind.I32, OperationKind.Eqz),
                OpCode.I64Eqz => (ValueKind.I64, OperationKind.Eqz),
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
        }

        public override void Handle(ref IntermediateContext context)
        {
            var popped = context.Pop();
            Debug.Assert(popped.Type == Type, $"Popped operand of type {popped.Type} in {Type}{Operation} instruction");

            var pushed = context.Push(ValueKind.I32);
            
            context.WriteFull($"{pushed} = {string.Format(EnumUtils.GetDescription(Operation), popped)}");
        }

        public enum OperationKind
        {
            [Description("{0} == 0")] Eqz,
        }
    }
}