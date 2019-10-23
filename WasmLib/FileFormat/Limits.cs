using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public struct Limits
    {
        public uint Min { get; private set; }
        public uint? Max { get; private set; }

        public Limits(uint min)
        {
            Min = min;
            Max = null;
        }

        public Limits(uint min, uint max)
        {
            Min = min;
            Max = max;
        }

        public static Limits Read(BinaryReader br)
        {
            return br.ReadBoolean()
                ? new Limits(br.ReadVarUint32(), br.ReadVarUint32())
                : new Limits(br.ReadVarUint32());
        }

        public override string ToString() => Max.HasValue ? $"[{Min}, {Max}]" : $"[{Min}, ϵ]";
    }
}
