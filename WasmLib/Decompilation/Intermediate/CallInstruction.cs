using System;
using System.Collections.Generic;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class CallInstruction : IntermediateInstruction
    {
        public string Name { get; }
        public FunctionSignature Signature { get; }
        
        public CallInstruction(WasmFile file, Instruction instruction)
        {
            if (instruction.OpCode != InstructionKind.Call) {
                throw new WrongInstructionPassedException(instruction, nameof(CallInstruction));
            }

            uint index = instruction.UIntOperand;
            Name = "fun_" + index.ToString("X8");

            // skipping signatures for imported functions
            Signature = file.FunctionTypes[file.Functions[index - file.ImportedFunctionCount]];
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            if (Signature.ReturnParameter.Length > 1) {
                throw new Exception("Not implemented");
            }
            
            var paramList = new List<Variable>();

            // pop parameters
            foreach (ValueKind param in Signature.Parameters) {
                var popped = context.Pop();
                Debug.Assert(popped.Type == param);
                paramList.Add(popped);
            }

            string parameters = string.Join(", ", paramList); // TODO: check order

            if (Signature.ReturnParameter.Length == 1) {
                var pushed = context.Push(Signature.ReturnParameter[0]);
                context.WriteFull($"{pushed} = {Name}({parameters})");
            }
            else {
                context.WriteFull($"{Name}({parameters})");
            }
        }
    }
}