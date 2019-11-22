using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib.Decompilation.SourceCode
{
    public class AssignmentExpression : IExpression
    {
        public IExpression BaseExpression { get; }
        public VariableReferenceExpression Reference { get; }
        public string Name { get; }

        public AssignmentExpression(IExpression baseExpression, ValueKind type, int index)
        {
            BaseExpression = baseExpression;
            Name = $"{EnumUtils.GetDescription(type)}_{index}";
            Reference = new VariableReferenceExpression(Name);
        }

        public string GetStringRepresentation() => $"{Reference.GetStringRepresentation()} = {BaseExpression.GetStringRepresentation()}";
    }
}