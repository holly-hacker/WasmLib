using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Global : IDeserializable
    {
        public GlobalType GlobalType => globalType ?? throw new UninitializedFieldException();
        public byte[] Expression => expression ?? throw new UninitializedFieldException();

        private GlobalType? globalType;
        private byte[]? expression;

        public void Read(BinaryReader br)
        {
            globalType = GlobalType.Read(br);
            expression = br.ReadExpression();
        }
    }
}
