using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public struct GlobalType
    {
        public bool Mutable { get; private set; }
        public ValueKind ValueKind { get; private set; }

        public GlobalType(ValueKind valueKind, bool mutable)
        {
            ValueKind = valueKind;
            Mutable = mutable;
        }

        public static GlobalType Read(BinaryReader br) => new GlobalType((ValueKind)br.ReadVarUint7(), br.ReadBoolean());

        public override string ToString() => Mutable ? $"mut {ValueKind}" : $"{ValueKind}";
    }
}
