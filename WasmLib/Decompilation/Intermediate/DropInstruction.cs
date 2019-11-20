using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public class DropInstruction : IntermediateInstruction
    {
        public override ValueKind[] PopTypes => new[] {ValueKind.Any};
        public override ValueKind[] PushTypes => new ValueKind[0];
        protected override string OperationStringFormat => "// drop {0}";
        public override bool IsPure => true;
    }
}