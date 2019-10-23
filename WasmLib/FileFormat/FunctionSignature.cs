using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class FunctionSignature : IDeserializable
    {
        public ValueKind[] Parameters { get; private set; } = new ValueKind[0];
        public ValueKind[] ReturnParameter { get; private set; } = new ValueKind[0];

        public void Read(BinaryReader br)
        {
            var type = br.ReadByte();
            Debug.Assert(type == 0x60);

            Parameters = br.ReadValueKindArray();
            ReturnParameter = br.ReadValueKindArray();
            Debug.Assert(ReturnParameter.Length == 1, $"Only 1 return parameter is supported, found {ReturnParameter.Length} in {nameof(FunctionSignature)}");
        }

        public override string ToString() => $"{(ReturnParameter.Any() ? ReturnParameter[0].ToString() : "void")}({string.Join(", ", Parameters.Select(x => x.ToString()))})";
    }
}
