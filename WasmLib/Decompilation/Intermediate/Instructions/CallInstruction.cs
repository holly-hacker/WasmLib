using System;
using System.Diagnostics;
using System.Linq;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class CallInstruction : IntermediateInstruction
    {
        public string? Name { get; }
        public bool IsIndirect { get; }
        public FunctionSignature Signature { get; }
        public override bool CanBeInlined => false; // only for readability
        public override StateKind ModifiesState => StateKind.All & ~StateKind.Locals; // TODO: could be optimized by checking if referenced function is pure
        public override StateKind ReadsState => StateKind.All & ~StateKind.Locals;

        public override ValueKind[] PopTypes => (IsIndirect ? new[] {ValueKind.I32} : new ValueKind[0]).Concat(Signature.Parameters.Reverse()).ToArray();
        public override ValueKind[] PushTypes => Signature.ReturnParameter.Reverse().ToArray();
        
        public CallInstruction(in Instruction instruction, WasmModule module)
        {
            uint index = instruction.UIntOperand;

            if (instruction.OpCode == OpCode.Call) {
                IsIndirect = false;
                Name = "fun_" + index.ToString("X8");

                if (index <= module.ImportedFunctionCount) {
                    var import = module.Imports.Where(x => x.Kind == ImportKind.TypeIndex).Skip((int)index).First();
                    
                    Debug.Assert(import.SignatureIndex != null);
                    Name = import.ModuleName + "." + import.ExportName;
                    Signature = module.FunctionTypes[import.SignatureIndex.Value];
                }
                else {
                    // skipping signatures for imported functions
                    Signature = module.FunctionTypes[module.Functions[index - module.ImportedFunctionCount]];
                }
            }
            else if (instruction.OpCode == OpCode.CallIndirect) {
                IsIndirect = true;
                Signature = module.FunctionTypes[index];
            }
            else {
                throw new WrongInstructionPassedException(instruction, nameof(CallInstruction));
            }
        }

        public override string OperationStringFormat {
            get {
                if (Signature.ReturnParameter.Length > 1) {
                    throw new Exception("Multiple return values not implemented");
                }

                string name = IsIndirect ? "ELEM[{0}]" : Name!;
                string parameters = string.Join(", ", Enumerable.Range(IsIndirect ? 1 : 0, PopCount - (IsIndirect ? 1 : 0)).Select(i => $"{{{i}}}"));

                return $"{name}({parameters})";
            }
        }
    }
}