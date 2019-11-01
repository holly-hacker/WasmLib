using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class FunctionSignature : IDeserializable
    {
        public ValueKind[] Parameters => parameters ?? throw new UninitializedFieldException();
        public ValueKind[] ReturnParameter => returnParameter ?? throw new UninitializedFieldException();

        private ValueKind[]? parameters, returnParameter;

        public void Read(BinaryReader br)
        {
            var type = br.ReadByte();
            Debug.Assert(type == 0x60);

            parameters = br.ReadValueKindArray();
            returnParameter = br.ReadValueKindArray();
            Debug.Assert(ReturnParameter.Length <= 1, $"Only 1 return parameter is supported, found {ReturnParameter.Length} in {nameof(FunctionSignature)}");
        }

        public override string ToString() => $"{(ReturnParameter.Any() ? ReturnParameter[0].ToString() : "void")}({string.Join(", ", Parameters.Select(x => x.ToString()))})";
        public string ToString(string functionName) => $"{(ReturnParameter.Any() ? ReturnParameter[0].ToString() : "void")} {functionName}({string.Join(", ", Parameters.Select(x => x.ToString()))})";
    }
}
