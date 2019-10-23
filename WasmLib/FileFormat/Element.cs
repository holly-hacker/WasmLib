using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Element : IDeserializable
    {
        public uint TableIndex { get; private set; }
        public byte[] Expression { get; private set; } = new byte[0];
        public uint[] FunctionIndices { get; private set; } = new uint[0];

        public void Read(BinaryReader br)
        {
            TableIndex = br.ReadVarUint32();
            Debug.Assert(TableIndex == 0, $"Only 1 table it allowed, but read table index of {TableIndex} in {nameof(Element)}");

            Expression = br.ReadExpression();
            FunctionIndices = br.ReadVarUint32Array();
        }

        public override string ToString() => $"(elem ({string.Join("-", Expression.Select(x => x.ToString("X2")))} {TableIndex}))";
    }
}
