using System;

namespace WasmLib.Decompilation.Intermediate
{
    [Flags]
    public enum StateKind : byte
    {
        None = 0,
        Locals = 1,
        Globals = 2,
        Memory = 4,
        All = 0xFF,
    }
}