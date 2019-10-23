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

        public ExportType Kind { get; private set; }

        public uint Index { get; private set; }

        public void Read(BinaryReader br)
        {
            Name = br.ReadIdentifier();
            Kind = (ExportType)br.ReadVarUint7();
            Index = br.ReadVarUint32();
        }

        public override string ToString() => Kind switch {
            ExportType.FuncIndex => $"(export \"{Name}\" (func {Index}))",
            ExportType.TableIndex => $"(export \"{Name}\" (table {Index}))",
            ExportType.MemoryIndex => $"(export \"{Name}\" (memory {Index}))",
            ExportType.GlobalIndex => $"(export \"{Name}\" (global {Index}))",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
