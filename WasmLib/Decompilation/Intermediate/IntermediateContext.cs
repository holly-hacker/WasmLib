using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public struct IntermediateContext
    {
        public int Indentation { get; private set; }
        public IReadOnlyList<ValueKind> Locals { get; }
        public IReadOnlyList<ValueKind> Globals { get; }
        public FunctionSignature Signature { get; }
        public WasmFile WasmFile { get; }
        private readonly StreamWriter streamWriter;
        
        public Stack<Variable> Stack { get; }
        public Stack<int> StackIndices { get; }
        private uint varCount;

        public IntermediateContext(FunctionBody function, FunctionSignature signature, WasmFile wasmFile, StreamWriter writer)
        {
            Indentation = 0;
            Locals = signature.Parameters.Concat(function.Locals).ToList();
            Signature = signature;
            WasmFile = wasmFile;

            var importGlobals = wasmFile.Imports
                .Where(x => x.Kind == ImportKind.GlobalType)
                .Select(x =>
                    x.GlobalType ??
                    throw new Exception($"{nameof(Import.GlobalType)} had no value, but {nameof(Import.Kind)} was {nameof(ImportKind.GlobalType)}"));
            var globals = wasmFile.Globals.Select(x => x.GlobalType); 
            Globals = importGlobals.Concat(globals).Select(x => x.ValueKind).ToList();
            streamWriter = writer;
            Stack = new Stack<Variable>();
            StackIndices = new Stack<int>();
            StackIndices.Push(0);
            varCount = 0;
        }

        public Variable Peek()
        {
            Debug.Assert(Stack.Any(), "Tried to peek a value from an empty stack");
            return Stack.Peek();
        }

        public Variable Pop()
        {
            Debug.Assert(Stack.Any(), "Tried to pop a value from an empty stack");
            Debug.Assert(Stack.Count > StackIndices.Peek(), $"Pop causes stack size to becomes less than {StackIndices.Count}, which is the minimum for this block");
            return Stack.Pop();
        }

        public Variable Push(ValueKind type)
        {
            Variable variable = Variable.Stack(type, varCount++);
            Stack.Push(variable);
            return variable;
        }

        public void EnterBlock()
        {
            Indent();
            StackIndices.Push(Stack.Count);
        }

        public void ExitBlock()
        {
            DeIndent();
            int previousStackSize = StackIndices.Pop();

            while (Stack.Count > previousStackSize) {
                Stack.Pop();
            }
        }

        public ValueKind GetLocalType(uint i) => Locals[(int)i];

        public ValueKind GetGlobalType(uint i) => Globals[(int)i];

        public void Indent() => Indentation++;
        
        public void DeIndent()
        {
            Indentation--;
            Debug.Assert(Indentation >= 0, "Tried to set indentation lower than zero");
        }

        public void WriteFull(string s)
        {
            #if DEBUG
            streamWriter.Write(Stack.Any() ? $"/* {Stack.Count,2} */" : "/*    */");
            #endif
            
            // NOTE: can split these up if needed for performance reasons (eg. to prevent string interp/concat on caller side)
            for (int i = 0; i < Indentation + 1; i++) {
                streamWriter.Write('\t');
            }
            
            streamWriter.Write(s);
            streamWriter.WriteLine();
            
            #if DEBUG
            streamWriter.Flush();
            #endif
        }
    }
}