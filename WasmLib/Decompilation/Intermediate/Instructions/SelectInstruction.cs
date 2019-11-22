using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class SelectInstruction : IntermediateInstruction
    {
        public override ValueKind[] PopTypes => new[] {ValueKind.I32, ValueKind.Any, ValueKind.Any}; // TODO: shouldn't be any
        public override ValueKind[] PushTypes => new[] {ValueKind.Any};

        public override bool IsPure => true;

        public override string OperationStringFormat => "{0} ? {2} : {1}";
    }
}