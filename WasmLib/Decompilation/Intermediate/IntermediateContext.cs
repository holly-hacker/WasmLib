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
        public FunctionSignature Signature { get; }
        public WasmModule WasmModule { get; }
        private readonly StreamWriter streamWriter;
        
        public Stack<Variable> Stack { get; }
        public Stack<int> StackIndices { get; }
        public bool RestOfBlockUnreachable { get; set; }

        private uint varCount;

        public IntermediateContext(FunctionSignature signature, WasmModule wasmModule, StreamWriter writer)
        {
            Indentation = 0;
            Signature = signature;
            WasmModule = wasmModule;
            streamWriter = writer;
            Stack = new Stack<Variable>();
            StackIndices = new Stack<int>();
            StackIndices.Push(0);
            varCount = 0;

            RestOfBlockUnreachable = false;
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

            if (!RestOfBlockUnreachable) {
                Debug.Assert(Stack.Count <= previousStackSize + 1);
            }

            RestOfBlockUnreachable = false;

            while (Stack.Count > previousStackSize) {
                Stack.Pop();
            }
        }

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
            
            #if DEBUG_FLUSH
            streamWriter.Flush();
            #endif
        }
    }
}