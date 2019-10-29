using System.IO;

namespace WasmLib.Decompilation
{
    public interface IDecompiler
    {
        void DecompileFunction(StreamWriter output, int functionIndex);
    }
}