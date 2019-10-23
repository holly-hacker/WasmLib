using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Global
    {
        public GlobalType GlobalType => globalType ?? throw new UninitializedFieldException();
        public byte[] Expression => expression ?? throw new UninitializedFieldException();

        private GlobalType? globalType;
        private byte[]? expression;

        public static Global Read(BinaryReader br)
        {
            return new Global {
                globalType = GlobalType.Read(br),
                expression = br.ReadExpression()
            };
        }
    }
}
