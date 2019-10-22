using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class TypeSection
    {
        public FunctionSignature[] FunctionTypes { get; private set; } = new FunctionSignature[0];

        public static TypeSection Empty => new TypeSection();

        internal static TypeSection Read(BinaryReader reader)
        {
            return new TypeSection {
                FunctionTypes = reader.ReadWasmArray<FunctionSignature>()
            };
        }
    }
}
