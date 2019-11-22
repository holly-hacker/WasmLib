namespace WasmLib.Decompilation.SourceCode
{
    public abstract class Expression
    {
        public abstract string GetStringRepresentation();

        public override string ToString() => GetStringRepresentation();
    }
}