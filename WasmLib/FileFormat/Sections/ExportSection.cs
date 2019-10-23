using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class ExportSection
    {
        public Export[] Exports { get; private set; } = new Export[0];

        public static ExportSection Empty => new ExportSection();

        internal static ExportSection Read(BinaryReader reader)
        {
            return new ExportSection {
                Exports = reader.ReadWasmArray<Export>()
            };
        }
    }
}
