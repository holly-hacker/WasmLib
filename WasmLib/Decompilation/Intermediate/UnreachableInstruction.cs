namespace WasmLib.Decompilation.Intermediate
{
    public class UnreachableInstruction : IntermediateInstruction
    {
        public override void Handle(ref IntermediateContext context)
        {
            context.WriteFull("// UNREACHABLE");
            context.JumpedOutOfBlock = true;
        }
    }
}