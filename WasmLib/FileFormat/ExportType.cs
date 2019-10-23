using System.ComponentModel;

namespace WasmLib.FileFormat
{
    public enum ExportType : byte
    {
        [Description("func")] FuncIndex,
        [Description("table")] TableIndex,
        [Description("memory")] MemoryIndex,
        [Description("global")] GlobalIndex,
    }
}
