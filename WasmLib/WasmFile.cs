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
                    Debug.Assert(size < stream.Length - stream.Position);

                    switch (type) {
                        case SectionType.Type:
                            file.TypeSection = TypeSection.Read(br);
                            break;
                        default:
                            throw new NotImplementedException(type.ToString());
                    }
                }
            }

            return file;
        }
    }
}
