using System.Linq;
using Rivers;
using WasmLib.Decompilation.Intermediate;
using WasmLib.Utils;

namespace WasmLib.Decompilation.SourceCode
{
    public class GenericExpression : IExpression
    {
        public IntermediateInstruction BaseInstruction { get; }
        public IExpression[]? Parameters { get; }
        public Graph? Block1 { get; }
        public Graph? Block2 { get; }

        public GenericExpression(IntermediateInstruction baseInstruction, IExpression[]? parameters = null, Graph? block1 = null, Graph? block2 = null)
        {
            BaseInstruction = baseInstruction;
            Parameters = parameters;
            Block1 = block1;
            Block2 = block2;
        }

        public string GetStringRepresentation() => BaseInstruction.OperationStringFormat.SafeFormat(Parameters?.Select(x => x.GetStringRepresentation()).ToArray());
    }
}