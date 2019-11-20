using Rivers;

namespace WasmLib.Decompilation.Intermediate
{
    public class InstructionNode : Node
    {
        public IntermediateInstruction Instruction { get; }

        public InstructionNode(IntermediateInstruction instruction, int idx) : base($"0x{idx:X}: {instruction}")
        {
            Instruction = instruction;
        }
    }
}