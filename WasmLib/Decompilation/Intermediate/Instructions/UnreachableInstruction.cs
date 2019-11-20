using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class UnreachableInstruction : IntermediateInstruction
    {
        public override ValueKind[] PopTypes => new ValueKind[0];
        public override ValueKind[] PushTypes => new ValueKind[0];

        protected override string OperationStringFormat => "// UNREACHABLE";
        public override bool RestOfBlockUnreachable => true;
    }
}