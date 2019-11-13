using WasmLib.FileFormat;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public readonly struct Variable
    {
        public ValueKind Type { get; }
        public readonly uint Index;

        private Variable(ValueKind type, uint index)
        {
            Type = type;
            Index = index;
        }

        public static Variable Stack(ValueKind type, uint index) => new Variable(type, index);

        public override string ToString() => $"var{Index}_{EnumUtils.GetDescription(Type)}";
    }
}