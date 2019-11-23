using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rivers;

namespace WasmLib.Decompilation.Intermediate.Graph
{
    public class InstructionNode : Node
    {
        public IntermediateInstruction Instruction { get; }
        public int Index { get; }
        public bool IsOrderImportant => Instruction.IsOrderImportant;

        public IEnumerable<ImpurityDependencyEdge> OutgoingImpurityEdges => OutgoingEdges.OfType<ImpurityDependencyEdge>();
        public IEnumerable<ImpurityDependencyEdge> IncomingImpurityEdges => IncomingEdges.OfType<ImpurityDependencyEdge>();
        public IEnumerable<StackVariableEdge> OutgoingVariableEdges => OutgoingEdges.OfType<StackVariableEdge>();
        public IEnumerable<StackVariableEdge> IncomingVariableEdges => IncomingEdges.OfType<StackVariableEdge>();
        public Rivers.Graph? Block1 { get; }
        public Rivers.Graph? Block2 { get; }

        public InstructionNode(IntermediateInstruction instruction, int idx, Rivers.Graph? block1 = null, Rivers.Graph? block2 = null) : base($"_{idx:X4}")
        {
            Instruction = instruction;
            Index = idx;
            Block1 = block1;
            Block2 = block2;
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

        public override string ToString() => Instruction.ToString();
    }
}