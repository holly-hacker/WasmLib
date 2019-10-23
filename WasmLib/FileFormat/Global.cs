using System.IO;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Global : IDeserializable
    {
        public GlobalType GlobalType => globalType ?? throw new UninitializedFieldException();
        public Instruction[] Expression => expression ?? throw new UninitializedFieldException();

        private GlobalType? globalType;
        private Instruction[]? expression;

        public void Read(BinaryReader br)
        {
            globalType = GlobalType.Read(br);
            expression = br.ReadExpression();
        }
    }
}
