using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib
{
    public class WasmFile
    {
        public int Version { get; private set; }

        public FunctionSignature[] FunctionTypes { get; private set; } = new FunctionSignature[0];
        public Import[] Imports { get; private set; } = new Import[0];
        public uint[] Functions { get; private set; } = new uint[0];
        public Global[] Globals { get; private set; } = new Global[0];
        public Export[] Exports { get; private set; } = new Export[0];
        public Element[] Elements { get; private set; } = new Element[0];
        public FunctionBody[] FunctionBodies { get; private set; } = new FunctionBody[0];
        public DataSegment[] DataSegments { get; private set; } = new DataSegment[0];

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
                    throw new Exception("Invalid magic, expected '\\0asm'");
                }

                file.Version = br.ReadInt32();
                Debug.Assert(file.Version == 1);

                while (stream.Position < stream.Length) {
                    var type = (SectionType)br.ReadVarUint7();
                    uint size = br.ReadVarUint32();
                    long oldPos = stream.Position;
                    Debug.Assert(size <= stream.Length - stream.Position);

                    switch (type) {
                        // TODO: custom
                        case SectionType.Type:
                            file.FunctionTypes = br.ReadWasmArray<FunctionSignature>();
                            break;
                        case SectionType.Import:
                            file.Imports = br.ReadWasmArray<Import>();
                            break;
                        case SectionType.Function:
                            file.Functions = br.ReadVarUint32Array();
                            break;
                        // TODO: table
                        // TODO: memory
                        case SectionType.Global:
                            file.Globals = br.ReadWasmArray<Global>();
                            break;
                        case SectionType.Export:
                            file.Exports = br.ReadWasmArray<Export>();
                            break;
                        // TODO: start
                        case SectionType.Element:
                            file.Elements = br.ReadWasmArray<Element>();
                            break;
                        case SectionType.Code:
                            file.FunctionBodies = br.ReadWasmArray<FunctionBody>();
                            break;
                        case SectionType.Data:
                            file.DataSegments = br.ReadWasmArray<DataSegment>();
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
