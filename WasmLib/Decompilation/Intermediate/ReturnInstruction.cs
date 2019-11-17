using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public class ReturnInstruction : IntermediateInstruction
    {
        private readonly FunctionSignature signature;

        public ReturnInstruction(FunctionSignature signature)
        {
            this.signature = signature;
        }

        public override ValueKind[] PopTypes => signature.ReturnParameter;
        public override ValueKind[] PushTypes => new ValueKind[0];
        public override bool RestOfBlockUnreachable => true;

        protected override string OperationStringFormat => signature.ReturnParameter.Length == 0 ? "return" : "return {0}";
    }
}