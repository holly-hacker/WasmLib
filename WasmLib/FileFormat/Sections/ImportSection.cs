using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class ImportSection
    {
        public Import[] Imports { get; private set; } = new Import[0];

        public static ImportSection Empty => new ImportSection();

        internal static ImportSection Read(BinaryReader reader)
        {
            return new ImportSection {
                Imports = reader.ReadWasmArray<Import>()
            };
        }
    }
}
