using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public struct Variable
    {
        public ValueKind Type { get; private set; }
        private uint index;

        public static Variable Stack(ValueKind type, uint index) => new Variable {
            Type = type,
            index = index,
        };

        public override string ToString() => $"var{index}_{EnumUtils.GetDescription(Type)}";
    }
}