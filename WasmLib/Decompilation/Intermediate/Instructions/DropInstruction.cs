using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class DropInstruction : IntermediateInstruction
    {
        public override ValueKind[] PopTypes => new[] {ValueKind.Any};
        public override ValueKind[] PushTypes => new ValueKind[0];
        public override string OperationStringFormat => "// drop {0}";
        public override bool IsPure => true;
        public override bool CanBeInlined => false; // inlined behavior would be part of the comment
    }
}