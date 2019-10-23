using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class DataSegment : IDeserializable
    {
        public uint MemoryIndex => memoryIndex ?? throw new UninitializedFieldException();
        public byte[] Expression => expression ?? throw new UninitializedFieldException();
        public byte[] Data => data ?? throw new UninitializedFieldException();

        private uint? memoryIndex;
        private byte[]? expression, data;

        public void Read(BinaryReader br)
        {
            memoryIndex = br.ReadVarUint32();
            expression = br.ReadExpression();
            data = br.ReadWasmByteArray();
        }
    }
}
