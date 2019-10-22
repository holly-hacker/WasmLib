using System;
using System.IO;
using System.Linq;
using System.Text;

namespace WasmLib
{
    public class WasmFile
    {
        public int Version { get; private set; }

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
            }

            return file;
        }
    }
}
