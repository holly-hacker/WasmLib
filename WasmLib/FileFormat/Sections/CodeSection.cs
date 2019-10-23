using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class CodeSection
    {
        public FunctionBody[] FunctionBodies { get; private set; } = new FunctionBody[0];

        public static CodeSection Empty => new CodeSection();

        internal static CodeSection Read(BinaryReader reader)
        {
            return new CodeSection {
                FunctionBodies = reader.ReadWasmArray<FunctionBody>()
            };
        }
    }
}
