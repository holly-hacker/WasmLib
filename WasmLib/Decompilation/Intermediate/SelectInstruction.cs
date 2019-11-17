using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public class SelectInstruction : IntermediateInstruction
    {
        public override ValueKind[] PopTypes => new[] {ValueKind.I32, ValueKind.Any, ValueKind.Any}; // TODO: shouldn't be any
        public override ValueKind[] PushTypes => new[] {ValueKind.Any};

        protected override string OperationStringFormat => "{0} = {1} ? {3} : {2}";
    }
}