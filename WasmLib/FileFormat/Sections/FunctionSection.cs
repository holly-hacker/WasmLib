using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class FunctionSection
    {
        public uint[] Functions { get; private set; } = new uint[0];

        public static FunctionSection Empty => new FunctionSection();

        internal static FunctionSection Read(BinaryReader reader)
        {
            return new FunctionSection {
                Functions = reader.ReadVarUint32Array(),
            };
        }
    }
}
