using System.Diagnostics;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public abstract class IntermediateInstruction
    {
        // TODO: abstract pop/pushtypes private for caching
        public abstract ValueKind[] PopTypes { get; }
        public abstract ValueKind[] PushTypes { get; }
        public virtual bool RestOfBlockUnreachable => false;
        public virtual bool ModifiesControlFlow => false;
        public virtual StateKind ModifiesState => StateKind.None;
        public virtual StateKind ReadsState => StateKind.None;
        public virtual bool CanInline => true;
        public virtual bool CanBeInlined => true; // only matters if PushTypes has items
        public virtual bool IsImplicit => false;

        public bool HasBlock => Block1 != null;
        
        public ControlBlock? Block1 { get; protected set; }
        public ControlBlock? Block2 { get; protected set; }
        
        public int PopCount => PopTypes.Length;
        public int PushCount => PushTypes.Length;

        public abstract string OperationStringFormat { get; }
        public virtual string? Comment => null;

        public override string ToString() => GetType().Name.Replace("Instruction", string.Empty);

        /// <summary>
        /// Determines whether the <paramref name="other"/> instruction stops
        /// the current instruction from being inlined.
        /// </summary>
        public bool HasConflictingState(IntermediateInstruction other, StateKind kinds = StateKind.All)
        {
            for (int i = 1; i < byte.MaxValue; i <<= 1) {
                StateKind kind = (StateKind)i;
                if (!kinds.HasFlag(kind)) {
                    continue;
                }
                
                bool thisRead = ReadsState.HasFlag(kind);
                bool thisWrite = ModifiesState.HasFlag(kind);
                bool otherRead = other.ReadsState.HasFlag(kind);
                bool otherWrite = other.ModifiesState.HasFlag(kind);

                bool thisAccess = thisRead || thisWrite;
                bool otherAccess = otherRead || otherWrite;

                if (!thisAccess || !otherAccess) {
                    continue;
                }

                if (!thisWrite && !otherWrite) {
                    continue;
                }

                Debug.Assert(thisRead && otherWrite || thisWrite && otherRead || thisWrite && otherWrite);
                return true;
            }

            return false;
        }
    }
}