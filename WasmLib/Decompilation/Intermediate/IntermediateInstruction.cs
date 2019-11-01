namespace WasmLib.Decompilation.Intermediate
{
    public abstract class IntermediateInstruction
    {
        public abstract void Handle(ref IntermediateContext context);
    }
}