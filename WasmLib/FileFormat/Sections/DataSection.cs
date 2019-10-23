using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class DataSection
    {
        public DataSegment[] DataSegments { get; private set; } = new DataSegment[0];

        public static DataSection Empty => new DataSection();

        internal static DataSection Read(BinaryReader reader)
        {
            return new DataSection {
                DataSegments = reader.ReadWasmArray<DataSegment>()
            };
        }
    }
}
