using System.Linq;
using WasmLib.Decompilation.Intermediate;
using WasmLib.Utils;

namespace WasmLib.Decompilation.SourceCode
{
    public class GenericExpression : IExpression
    {
        public IntermediateInstruction BaseInstruction { get; }
        public IExpression[]? Parameters { get; }

        public GenericExpression(IntermediateInstruction baseInstruction, IExpression[]? parameters = null)
        {
            BaseInstruction = baseInstruction;
            Parameters = parameters;
        }

        public string GetStringRepresentation() => BaseInstruction.OperationStringFormat.SafeFormat(Parameters?.Select(x => x.GetStringRepresentation()).ToArray());
    }
}