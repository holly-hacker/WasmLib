using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class VariableInstruction : IntermediateInstruction
    {
        public TargetKind Target { get; }
        public ActionKind Action { get; }
        public uint Index { get; }
        public ValueKind Type => Target == TargetKind.Local ? GetLocal(Index) : GetGlobal(Index);
        public override StateKind ModifiesState => Action == ActionKind.Get ? StateKind.None : Target == TargetKind.Global ? StateKind.Globals : StateKind.Locals;
        public override StateKind ReadsState => Action == ActionKind.Set ? StateKind.None : Target == TargetKind.Global ? StateKind.Globals : StateKind.Locals;
        public override bool CanBeInlined => Action != ActionKind.Tee; // would cause assignments in comparisons

        private readonly WasmModule module;
        private readonly FunctionBody body;
        private readonly FunctionSignature signature;

        private IEnumerable<ValueKind> Locals => signature.Parameters.Concat(body.Locals);
        private IEnumerable<ValueKind> Globals => module.ImportedGlobals.Concat(module.Globals.Select(x => x.GlobalType)).Select(x => x.ValueKind);

        public VariableInstruction(in Instruction instruction, WasmModule module, FunctionBody body, FunctionSignature signature)
        {
            this.module = module;
            this.body = body;
            this.signature = signature;
            (Target, Action) = instruction.OpCode switch {
                OpCode.LocalGet => (TargetKind.Local, ActionKind.Get),
                OpCode.LocalSet => (TargetKind.Local, ActionKind.Set),
                OpCode.LocalTee => (TargetKind.Local, ActionKind.Tee),
                OpCode.GlobalGet => (TargetKind.Global, ActionKind.Get),
                OpCode.GlobalSet => (TargetKind.Global, ActionKind.Set),
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
            Index = instruction.UIntOperand;
        }
        
        public override ValueKind[] PopTypes => Action == ActionKind.Get ? new ValueKind[0] : new[] {Type};
        public override ValueKind[] PushTypes => Action == ActionKind.Set ? new ValueKind[0] : new[] {Type};

        public override string OperationStringFormat {
            get {
                Debug.Assert((Target == TargetKind.Local ? Locals.Count() : Globals.Count()) > Index);
                
                // NOTE: perhaps this naming stuff can be moved to a central location
                string varName = Target == TargetKind.Local ? $"local[{Index - signature.Parameters.Length}]" : $"global[{Index}]";

                if (Target == TargetKind.Local && Index < signature.Parameters.Length) {
                    varName = $"param_{Index}";
                }
                
                return Action switch {
                    ActionKind.Get => varName,
                    ActionKind.Set => varName + " = {0}",
                    ActionKind.Tee => varName + " = {0}",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        private ValueKind GetLocal(uint idx) => Locals.Skip((int)idx).First();
        private ValueKind GetGlobal(uint idx) => Globals.Skip((int)idx).First();

        public override string ToString() => $"{Action} {Target}[{Index}]";

        public enum TargetKind
        {
            Local,
            Global,
        }

        public enum ActionKind
        {
            Get,
            Set,
            Tee,
        }
    }
}