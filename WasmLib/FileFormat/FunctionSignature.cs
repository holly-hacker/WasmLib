using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class FunctionSignature : IDeserializable
    {
        public WasmValueType[] Parameters { get; private set; } = new WasmValueType[0];
        public WasmValueType[] ReturnParameter { get; private set; } = new WasmValueType[0];

        public void Read(BinaryReader br)
        {
            var type = br.ReadByte();
            Debug.Assert(type == 0x60);

            Parameters = br.ReadValueTypeArray();
            ReturnParameter = br.ReadValueTypeArray();
        }

        public override string ToString() => $"{(ReturnParameter.Any() ? ReturnParameter[0].ToString() : "void")}({string.Join(", ", Parameters.Select(x => x.ToString()))})";
    }
}
