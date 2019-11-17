using System.ComponentModel;

namespace WasmLib.FileFormat
{
    public enum ValueKind : byte
    {
        [Description("i32")] I32 = 0x7f,
        [Description("i64")] I64 = 0x7e,
        [Description("f32")] F32 = 0x7d,
        [Description("f64")] F64 = 0x7c,

        /// <remarks> Only used as block type </remarks>
        Empty = 0x40,
        
        /// <remarks> Only used internally </remarks>
        Any = 0xFF,
    }
}
