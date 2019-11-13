using System;
using System.Collections.Generic;
using System.IO;
using WasmLib.Decompilation.Intermediate;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation
{
    public class IntermediateRepresentationDecompiler : IDecompiler
    {
        public WasmModule WasmModule { get; }

        public IntermediateRepresentationDecompiler(WasmModule wasmModule)
        {
            WasmModule = wasmModule;
        }

        public void DecompileFunction(StreamWriter output, int functionIndex)
        {
            FunctionBody body = WasmModule.FunctionBodies[functionIndex];
            FunctionSignature signature = WasmModule.FunctionTypes[WasmModule.Functions[functionIndex]];
            
            // get IR
            var context = new IntermediateContext(body, signature, WasmModule, output);
            List<IntermediateInstruction> instructions = new IntermediateConverter(WasmModule, body).Convert();

            output.Write(signature.ToString($"fun_{functionIndex:X8}"));
            output.WriteLine(" {");

            // write all IR while simulating the stack
            foreach (IntermediateInstruction instruction in instructions) {
                instruction.Handle(ref context);
            }
            
            // write return value, if needed
            if (signature.ReturnParameter.Length != 0) {
                new ImplicitReturnInstruction().Handle(ref context);
            }
            
            output.WriteLine("}");

            if (context.Indentation != 0) {
                throw new Exception("Function body has unbalanced indentation");
            }

            if (context.Stack.Count != 0) {
                throw new Exception($"Unbalanced stack, found {context.Stack.Count} remaining values");
            }
            
            output.WriteLine();
        }
    }
}