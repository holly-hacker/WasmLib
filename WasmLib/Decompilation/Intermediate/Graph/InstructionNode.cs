using System.Diagnostics;
using Rivers;

namespace WasmLib.Decompilation.Intermediate.Graph
{
    public class InstructionNode : Node
    {
        public IntermediateInstruction Instruction { get; }

        public InstructionNode(IntermediateInstruction instruction, int idx) : base($"_{idx:X4}")
        {
            Instruction = instruction;
            AddUserData();
        }

        [Conditional("DEBUG")]
        private void AddUserData()
        {
            // BUG: Rivers will not quote single words, which can cause parsers to fail if the output starts with a number
            // see Washi1337/Rivers#7
            string instructionString = Instruction.ToString();
            UserData["label"] = char.IsDigit(instructionString[0]) && !instructionString.Contains(" ")
                ? $" {instructionString} "
                : instructionString;
        }
    }
}