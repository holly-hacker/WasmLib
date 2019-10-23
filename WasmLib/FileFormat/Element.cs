using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class Element : IDeserializable
    {
        public uint TableIndex => tableIndex ?? throw new UninitializedFieldException();
        public Instruction[] Expression => expression ?? throw new UninitializedFieldException();
        public uint[] FunctionIndices => functionIndices ?? throw new UninitializedFieldException();

        private uint? tableIndex;
        private Instruction[]? expression;
        private uint[]? functionIndices;

        public void Read(BinaryReader br)
        {
            tableIndex = br.ReadVarUint32();
            Debug.Assert(TableIndex == 0, $"Only 1 table it allowed, but read table index of {TableIndex} in {nameof(Element)}");

            expression = br.ReadExpression();
            functionIndices = br.ReadVarUint32Array();
        }

        public override string ToString() => $"(elem ({string.Join(";", Expression.Select(x => x.ToString()))} {TableIndex}))";
    }
}
