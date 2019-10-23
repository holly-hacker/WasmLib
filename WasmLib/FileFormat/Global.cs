using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public struct Global
    {
        public GlobalType GlobalType { get; private set; }
        public byte[] Expression { get; private set; }

        public static Global Read(BinaryReader br)
        {
            return new Global {
                GlobalType = GlobalType.Read(br),
                Expression = br.ReadExpression()
            };
        }
    }
}
