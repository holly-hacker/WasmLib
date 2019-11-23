using Rivers;
using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib.Decompilation.SourceCode
{
    public class AssignmentExpression : IExpression, IHasBlocks
    {
        public IExpression BaseExpression { get; }
        public VariableReferenceExpression Reference { get; }
        public string Name { get; }
        public Graph? Block1 => (BaseExpression as IHasBlocks)?.Block1;
        public Graph? Block2 => (BaseExpression as IHasBlocks)?.Block2;

        public AssignmentExpression(IExpression baseExpression, ValueKind type, int index)
        {
            BaseExpression = baseExpression;
            Name = $"{EnumUtils.GetDescription(type)}_{index}";
            Reference = new VariableReferenceExpression(Name);
        }

        public string GetStringRepresentation() => $"{Reference.GetStringRepresentation()} = {BaseExpression.GetStringRepresentation()}";
    }
}