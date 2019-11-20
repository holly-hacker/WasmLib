using System.Diagnostics;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate.Graph
{
    public class StackVariableEdge : InstructionEdge
    {
        public ValueKind Type { get; }

        public StackVariableEdge(InstructionNode source, InstructionNode target, ValueKind type) : base(source, target)
        {
            Type = type;
            AddUserData();
        }

        [Conditional("DEBUG")]
        private void AddUserData()
        {
            UserData["color"] = "red";
            UserData["label"] = Type;
        }
    }
}