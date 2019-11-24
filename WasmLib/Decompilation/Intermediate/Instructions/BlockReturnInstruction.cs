using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class BlockReturnInstruction : IntermediateInstruction
    {
        private readonly ValueKind type;

        public override ValueKind[] PopTypes => new[] {type};
        public override ValueKind[] PushTypes => new ValueKind[0];

        public override bool RestOfBlockUnreachable => true;
        public override bool ModifiesControlFlow => true;
        public override bool IsImplicit => true;

        public BlockReturnInstruction(ValueKind type)
        {
            this.type = type;
        }

        public override string OperationStringFormat => "block_return {0}";
    }
}