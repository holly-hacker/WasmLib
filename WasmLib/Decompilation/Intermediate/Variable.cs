using WasmLib.FileFormat;

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

        public override string ToString() => $"var_{index}_{Type}";
    }
}