using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class UnreachableInstruction : IntermediateInstruction
    {
        public override ValueKind[] PopTypes => new ValueKind[0];
        public override ValueKind[] PushTypes => new ValueKind[0];

        public override bool RestOfBlockUnreachable => true;
        public override bool ModifiesControlFlow => true;
        public override bool CanBeInlined => false;

        public override string OperationStringFormat => "// UNREACHABLE";
    }
}