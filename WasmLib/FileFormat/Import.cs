using System;
using System.Diagnostics;
using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Import : IDeserializable
    {
        public string ModuleName => moduleName ?? throw new UninitializedFieldException();
        public string ExportName => exportName ?? throw new UninitializedFieldException();
        public ImportKind Kind => kind ?? throw new UninitializedFieldException();

        private string? moduleName, exportName;
        private ImportKind? kind;

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportKind.TypeIndex"/> </remarks>
        public uint? SignatureIndex { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportKind.TableType"/> </remarks>
        public Limits? TableSize { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportKind.MemoryType"/> </remarks>
        public Limits? MemorySize { get; private set; }

        /// <remarks> When <see cref="Kind"/> is <see cref="ImportKind.GlobalType"/> </remarks>
        public GlobalType? GlobalType { get; private set; }

        public void Read(BinaryReader br)
        {
            moduleName = br.ReadIdentifier();
            exportName = br.ReadIdentifier();
            kind = (ImportKind)br.ReadVarUint7();

            switch (Kind) {
                case ImportKind.TypeIndex:
                    SignatureIndex = br.ReadVarUint32();
                    break;
                case ImportKind.TableType:
                    byte elementType = br.ReadVarUint7();
                    Debug.Assert(elementType == 0x70);
                    TableSize = Limits.Read(br);
                    break;
                case ImportKind.MemoryType:
                    MemorySize = Limits.Read(br);
                    break;
                case ImportKind.GlobalType:
                    GlobalType = FileFormat.GlobalType.Read(br);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString() => Kind switch {
            ImportKind.TypeIndex => $"(import \"{ModuleName}\" \"{ExportName}\" (func (type {SignatureIndex})))",
            ImportKind.TableType => $"(import \"{ModuleName}\" \"{ExportName}\" (table {TableSize?.Min} {TableSize?.Max}))",
            ImportKind.MemoryType => $"(import \"{ModuleName}\" \"{ExportName}\" (memory {MemorySize?.Min} {MemorySize?.Max}))",
            ImportKind.GlobalType => $"(import \"{ModuleName}\" \"{ExportName}\" (global {GlobalType}))",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
