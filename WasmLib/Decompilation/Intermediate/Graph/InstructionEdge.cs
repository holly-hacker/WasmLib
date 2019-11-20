using Rivers;

namespace WasmLib.Decompilation.Intermediate.Graph
{
    public abstract class InstructionEdge : Edge
    {
        public new InstructionNode Source => (InstructionNode)base.Source;
        public new InstructionNode Target => (InstructionNode)base.Target;

        protected InstructionEdge(InstructionNode source, InstructionNode target) : base(source, target) { }
    }
}