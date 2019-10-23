using System;
using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Export : IDeserializable
    {
        public string Name => name ?? throw new UninitializedFieldException();
        public ExportKind Kind => kind ?? throw new UninitializedFieldException();
        public uint Index => index ?? throw new UninitializedFieldException();

        private string? name;
        private ExportKind? kind;
        private uint? index;

        public void Read(BinaryReader br)
        {
            name = br.ReadIdentifier();
            kind = (ExportKind)br.ReadVarUint7();
            index = br.ReadVarUint32();
        }

        public override string ToString() => Kind switch {
            ExportKind.FuncIndex => $"(export \"{Name}\" (func {Index}))",
            ExportKind.TableIndex => $"(export \"{Name}\" (table {Index}))",
            ExportKind.MemoryIndex => $"(export \"{Name}\" (memory {Index}))",
            ExportKind.GlobalIndex => $"(export \"{Name}\" (global {Index}))",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
