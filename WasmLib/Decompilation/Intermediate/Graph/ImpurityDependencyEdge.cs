using System.Diagnostics;

namespace WasmLib.Decompilation.Intermediate.Graph
{
    public class ImpurityDependencyEdge : InstructionEdge
    {
        public ImpurityDependencyEdge(InstructionNode source, InstructionNode target) : base(source, target)
        {
            AddUserData();
        }

        [Conditional("DEBUG")]
        private void AddUserData()
        {
            UserData["style"] = "dotted";
            UserData["color"] = "blue";
        }
    }
}