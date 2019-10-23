using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public struct GlobalType
    {
        public bool Mutable { get; private set; }
        public WasmValueType ValueType { get; private set; }

        public GlobalType(WasmValueType valueType, bool mutable)
        {
            ValueType = valueType;
            Mutable = mutable;
        }

        public static GlobalType Read(BinaryReader br) => new GlobalType((WasmValueType)br.ReadVarUint7(), br.ReadBoolean());

        public override string ToString() => Mutable ? $"mut {ValueType}" : $"{ValueType}";
    }
}
