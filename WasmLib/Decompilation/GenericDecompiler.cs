using System.IO;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation
{
    public class GenericDecompiler : IDecompiler
    {
        public WasmModule WasmModule { get; }

        public GenericDecompiler(WasmModule wasmModule)
        {
            WasmModule = wasmModule;
        }
        
        public void DecompileFunction(StreamWriter output, int functionIndex)
        {
            FunctionBody body = WasmModule.FunctionBodies[functionIndex];
            FunctionSignature signature = WasmModule.FunctionTypes[WasmModule.Functions[functionIndex]];
            
            output.WriteLine(signature);
        }
    }
}