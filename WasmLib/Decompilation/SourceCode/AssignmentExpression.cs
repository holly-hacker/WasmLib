using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib.Decompilation.SourceCode
{
    public class AssignmentExpression : Expression
    {
        public Expression BaseExpression { get; }
        public VariableReferenceExpression Reference { get; }
        public string Name { get; }

        public AssignmentExpression(Expression baseExpression, ValueKind type, int index)
        {
            BaseExpression = baseExpression;
            Name = $"{EnumUtils.GetDescription(type)}_{index}";
            Reference = new VariableReferenceExpression(Name);
        }

        public override string GetStringRepresentation() => $"{Reference.GetStringRepresentation()} = {BaseExpression.GetStringRepresentation()}";
    }
}