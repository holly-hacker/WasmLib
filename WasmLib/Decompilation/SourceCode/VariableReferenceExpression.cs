namespace WasmLib.Decompilation.SourceCode
{
    public class VariableReferenceExpression : IExpression
    {
        public string Name { get; }

        public VariableReferenceExpression(string name)
        {
            Name = name;
        }

        public string GetStringRepresentation() => Name;
    }
}