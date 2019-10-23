using System.ComponentModel;

namespace WasmLib.FileFormat
{
    public enum ImportKind : byte
    {
        [Description("func")] TypeIndex,
        [Description("table")] TableType,
        [Description("memory")] MemoryType,
        [Description("global")] GlobalType,
    }
}
