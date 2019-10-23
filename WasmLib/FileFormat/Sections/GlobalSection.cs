using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class GlobalSection
    {
        public Global[] Globals { get; private set; } = new Global[0];

        public static GlobalSection Empty => new GlobalSection();

        internal static GlobalSection Read(BinaryReader reader)
        {
            var read = new GlobalSection {
                Globals = reader.ReadWasmArray<Global>()
            };

            return read;
        }
    }
}
