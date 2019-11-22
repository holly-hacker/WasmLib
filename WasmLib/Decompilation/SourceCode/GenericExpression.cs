using WasmLib.Decompilation.Intermediate;
using WasmLib.Utils;

namespace WasmLib.Decompilation.SourceCode
{
    public class GenericExpression : Expression
    {
        public IntermediateInstruction BaseInstruction { get; }
        public Expression[]? Parameters { get; }

        public GenericExpression(IntermediateInstruction baseInstruction, Expression[]? parameters = null)
        {
            BaseInstruction = baseInstruction;
            Parameters = parameters;
        }

        public override string GetStringRepresentation() => BaseInstruction.OperationStringFormat.SafeFormat(Parameters);
    }
}