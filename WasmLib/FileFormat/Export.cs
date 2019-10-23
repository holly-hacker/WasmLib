using System;
using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Export : IDeserializable
    {
        public string Name
        {
            get => name ?? throw new Exception($"Tried to read {nameof(Name)} before it was assigned");
            private set => name = value;
        }

        private string? name;

        public ExportKind Kind { get; private set; }

        public uint Index { get; private set; }

        public void Read(BinaryReader br)
        {
            Name = br.ReadIdentifier();
            Kind = (ExportKind)br.ReadVarUint7();
            Index = br.ReadVarUint32();
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
