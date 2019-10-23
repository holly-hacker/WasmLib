using System.IO;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class DataSegment : IDeserializable
    {
        public uint MemoryIndex => memoryIndex ?? throw new UninitializedFieldException();
        public Instruction[] Expression => expression ?? throw new UninitializedFieldException();
        public byte[] Data => data ?? throw new UninitializedFieldException();

        private uint? memoryIndex;
        private Instruction[]? expression;
        private byte[]? data;

        public void Read(BinaryReader br)
        {
            memoryIndex = br.ReadVarUint32();
            expression = br.ReadExpression();
            data = br.ReadWasmByteArray();
        }
    }
}
