using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public abstract class IntermediateInstruction
    {
        // TODO: abstract pop/pushtypes private for caching
        public abstract ValueKind[] PopTypes { get; }
        public abstract ValueKind[] PushTypes { get; }
        public virtual bool RestOfBlockUnreachable => false;
        public virtual bool IsPure => false;
        public virtual bool IsImplicit => false;

        public bool HasBlock => Block1 != null;
        
        public ControlBlock? Block1 { get; protected set; }
        public ControlBlock? Block2 { get; protected set; }
        
        public int PopCount => PopTypes.Length;
        public int PushCount => PushTypes.Length;

        public abstract string OperationStringFormat { get; }
        public virtual string? Comment => null;

        public override string ToString() => GetType().Name.Replace("Instruction", string.Empty);
    }
}