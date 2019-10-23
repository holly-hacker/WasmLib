using System;
using System.Diagnostics;
using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Import : IDeserializable
    {
        public string ModuleName
        {
            get => moduleName ?? throw new Exception($"Tried to read {nameof(ModuleName)} before it was assigned");
            private set => moduleName = value;
        }

        public string ExportName
        {
            get => exportName ?? throw new Exception($"Tried to read {nameof(ExportName)} before it was assigned");
            private set => exportName = value;
        }

        private string? moduleName, exportName;

        public ImportType Kind { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportType.TypeIndex"/> </remarks>
        public uint? SignatureIndex { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportType.TableType"/> </remarks>
        public Limits? TableSize { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportType.MemoryType"/> </remarks>
        public Limits? MemorySize { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportType.GlobalType"/> </remarks>
        public GlobalValue? GlobalType { get; private set; }

        public void Read(BinaryReader br)
        {
            ModuleName = br.ReadIdentifier();
            ExportName = br.ReadIdentifier();
            Kind = (ImportType)br.ReadVarUint7();

            switch (Kind) {
                case ImportType.TypeIndex:
                    SignatureIndex = br.ReadVarUint32();
                    break;
                case ImportType.TableType:
                    byte elementType = br.ReadVarUint7();
                    Debug.Assert(elementType == 0x70);
                    TableSize = Limits.Read(br);
                    break;
                case ImportType.MemoryType:
                    MemorySize = Limits.Read(br);
                    break;
                case ImportType.GlobalType:
                    GlobalType = GlobalValue.Read(br);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString() => Kind switch {
            ImportType.TypeIndex => $"(import \"{ModuleName}\" \"{ExportName}\" (func (type {SignatureIndex})))",
            ImportType.TableType => $"(import \"{ModuleName}\" \"{ExportName}\" (table {TableSize?.Min} {TableSize?.Max}))",
            ImportType.MemoryType => $"(import \"{ModuleName}\" \"{ExportName}\" (memory {MemorySize?.Min} {MemorySize?.Max}))",
            ImportType.GlobalType => $"(import \"{ModuleName}\" \"{ExportName}\" (global {GlobalType}))",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
