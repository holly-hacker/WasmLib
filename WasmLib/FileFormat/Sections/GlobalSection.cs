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
                Globals = new Global[reader.ReadVarUint32()]
            };

            for (int i = 0; i < read.Globals.Length; i++) {
                read.Globals[i] = Global.Read(reader);
            }

            return read;
        }
    }
}
