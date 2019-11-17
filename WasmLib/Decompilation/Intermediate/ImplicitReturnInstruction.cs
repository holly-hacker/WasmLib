using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    /// <remarks>
    /// This is a pseudo-instruction to handle implicit returns at the end of a function
    /// </remarks>
    public class ImplicitReturnInstruction : ReturnInstruction
    {
        public ImplicitReturnInstruction(FunctionSignature signature) : base(signature) { }
    }
}