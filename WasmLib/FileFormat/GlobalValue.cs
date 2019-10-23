using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public struct GlobalValue
    {
        public bool Mutable { get; private set; }
        public WasmValueType ValueType { get; private set; }

        public GlobalValue(WasmValueType valueType, bool mutable)
        {
            ValueType = valueType;
            Mutable = mutable;
        }

        public static GlobalValue Read(BinaryReader br) => new GlobalValue((WasmValueType)br.ReadVarUint7(), br.ReadBoolean());

        public override string ToString() => Mutable ? $"mut {ValueType}" : $"{ValueType}";
    }
}
