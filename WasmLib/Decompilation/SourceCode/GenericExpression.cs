using System.Linq;
using WasmLib.Decompilation.Intermediate;

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

        public override string GetStringRepresentation() => Parameters is null
            ? BaseInstruction.ToString()
            : $"{BaseInstruction}({string.Join(", ", Parameters.Reverse().Select(x => x.GetStringRepresentation()))})";
    }
}