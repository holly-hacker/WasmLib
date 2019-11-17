using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class VariableInstruction : IntermediateInstruction
    {
        public TargetKind Target { get; }
        public ActionKind Action { get; }
        public uint Index { get; }
        public ValueKind Type => Target == TargetKind.Local ? GetLocal(Index) : GetGlobal(Index);

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

        protected override string OperationStringFormat {
            get {
                Debug.Assert((Target == TargetKind.Local ? Locals.Count() : Globals.Count()) > Index);
                return Target switch {
                    TargetKind.Local => (Action switch {
                        ActionKind.Get => $"{{0}} = local[{Index}]",
                        ActionKind.Set => $"local[{Index}] = {{0}}",
                        ActionKind.Tee => $"{{0}} = local[{Index}] = {{1}}",
                        _ => throw new ArgumentOutOfRangeException()
                    }),
                    TargetKind.Global => (Action switch {
                        ActionKind.Get => $"{{0}} = global[{Index}]",
                        ActionKind.Set => $"global[{Index}] = {{0}}",
                        _ => throw new ArgumentOutOfRangeException()
                    }),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        private ValueKind GetLocal(uint idx) => Locals.Skip((int)idx).First();
        private ValueKind GetGlobal(uint idx) => Globals.Skip((int)idx).First();

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