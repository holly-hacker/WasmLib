using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WasmLib.FileFormat.Sections;
using WasmLib.Utils;

namespace WasmLib
{
    public class WasmFile
    {
        public int Version { get; private set; }

        public TypeSection TypeSection { get; private set; } = TypeSection.Empty;
        public ImportSection ImportSection { get; private set; } = ImportSection.Empty;
        public FunctionSection FunctionSection { get; private set; } = FunctionSection.Empty;
        public GlobalSection GlobalSection { get; private set; } = GlobalSection.Empty;
        public ExportSection ExportSection { get; private set; } = ExportSection.Empty;
        public ElementSection ElementSection { get; private set; } = ElementSection.Empty;
        public CodeSection CodeSection { get; private set; } = CodeSection.Empty;
        public DataSection DataSection { get; private set; } = DataSection.Empty;

        private WasmFile() { }

        public static WasmFile Read(string path)
        {
            using var fs = File.OpenRead(path);
            return Read(fs);
        }

        public static WasmFile Read(Stream stream)
        {
            var file = new WasmFile();

            using (var br = new BinaryReader(stream, Encoding.UTF8, true)) {
                var magic = br.ReadBytes(4);
                if (!magic.SequenceEqual(Encoding.ASCII.GetBytes("\0asm"))) {
                    throw new Exception("Invalid magic, expected ''\\0asm'");
                }

                file.Version = br.ReadInt32();
                Debug.Assert(file.Version == 1);

                while (stream.Position < stream.Length) {
                    var type = (SectionType)br.ReadVarUint7();
                    uint size = br.ReadVarUint32();
                    long oldPos = stream.Position;
                    Debug.Assert(size <= stream.Length - stream.Position);

                    switch (type) {
                        case SectionType.Type:
                            file.TypeSection = TypeSection.Read(br);
                            break;
                        case SectionType.Import:
                            file.ImportSection = ImportSection.Read(br);
                            break;
                        case SectionType.Function:
                            file.FunctionSection = FunctionSection.Read(br);
                            break;
                        // TODO: table
                        // TODO: memory
                        case SectionType.Global:
                            file.GlobalSection = GlobalSection.Read(br);
                            break;
                        case SectionType.Export:
                            file.ExportSection = ExportSection.Read(br);
                            break;
                        // TODO: start
                        case SectionType.Element:
                            file.ElementSection = ElementSection.Read(br);
                            break;
                        case SectionType.Code:
                            file.CodeSection = CodeSection.Read(br);
                            break;
                        case SectionType.Data:
                            file.DataSection = DataSection.Read(br);
                            break;
                        default:
                            throw new NotImplementedException(type.ToString());
                    }

                    Debug.Assert(oldPos + size == stream.Position, $"Section size was {size}, but read {stream.Position - oldPos} bytes");
                }
            }

            return file;
        }
    }
}
