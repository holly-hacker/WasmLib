namespace WasmLib.Decompilation.Intermediate
{
    public class DropInstruction : IntermediateInstruction
    {
        public override void Handle(ref IntermediateContext context)
        {
            context.WriteFull($"// drop {context.Pop()}");
        }
    }
}