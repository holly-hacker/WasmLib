using Rivers;

namespace WasmLib.Decompilation.Intermediate
{
    public class InstructionNode : Node
    {
        private readonly IntermediateInstruction instruction;

        public InstructionNode(IntermediateInstruction instruction, int idx) : base($"0x{idx:X}: {instruction}")
        {
            this.instruction = instruction;
        }
    }
}