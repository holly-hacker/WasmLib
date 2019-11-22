namespace WasmLib.Decompilation.SourceCode
{
    public class VariableReferenceExpression : Expression
    {
        public string Name { get; }

        public VariableReferenceExpression(string name)
        {
            Name = name;
        }

        public override string GetStringRepresentation() => Name;
    }
}