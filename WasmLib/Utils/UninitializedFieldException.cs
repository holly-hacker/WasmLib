using System;
using System.Runtime.CompilerServices;

namespace WasmLib.Utils
{
    public class UninitializedFieldException : Exception
    {
        public UninitializedFieldException([CallerMemberName] string? fieldName = null) : base($"Tried to read {fieldName ?? "a field"} before it was assigned") { }
    }
}
