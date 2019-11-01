using System;
using System.Diagnostics;
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

        public VariableInstruction(in Instruction instruction)
        {
            (Target, Action) = instruction.Opcode switch {
                InstructionKind.LocalGet => (TargetKind.Local, ActionKind.Get),
                InstructionKind.LocalSet => (TargetKind.Local, ActionKind.Set),
                InstructionKind.LocalTee => (TargetKind.Local, ActionKind.Tee),
                InstructionKind.GlobalGet => (TargetKind.Global, ActionKind.Get),
                InstructionKind.GlobalSet => (TargetKind.Global, ActionKind.Set),
                _ => throw new WrongInstructionPassedException(instruction, nameof(VariableInstruction)),
            };
            Index = instruction.UIntOperand;
        }
        
        public VariableInstruction(TargetKind target, ActionKind action)
        {
            if (target != TargetKind.Global && action == ActionKind.Tee) {
                throw new ArgumentException("Tried creating IR instruction for global.tee, which does not exist");
            }

            Target = target;
            Action = action;
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            // NOTE: could make this a bit more DRY
            // TODO: make output more clear regarding locals and arguments
            if (Target == TargetKind.Local) {
                Debug.Assert(context.Locals.Count > Index);
                ValueKind local = context.GetLocalType(Index);

                if (Action == ActionKind.Get) {
                    var pushed = context.Push(local);
                    context.WriteFull($"{pushed} = local[{Index}]");
                }
                else if (Action == ActionKind.Set) {
                    var popped = context.Pop();
                    Debug.Assert(local == popped.Type);
                    context.WriteFull($"local[{Index}] = {popped}");
                }
                else if (Action == ActionKind.Tee) {
                    var peeked = context.Stack.Peek();
                    Debug.Assert(local == peeked.Type);
                    context.WriteFull($"local[{Index}] = {peeked}");
                }
            }
            else if (Target == TargetKind.Global) {
                Debug.Assert(context.Globals.Count > Index);
                ValueKind global = context.GetGlobalType(Index);

                if (Action == ActionKind.Get) {
                    var pushed = context.Push(global);
                    context.WriteFull($"{pushed} = global[{Index}]");
                }
                else if (Action == ActionKind.Set) {
                    // NOTE: could check for mutability of global
                    var popped = context.Pop();
                    Debug.Assert(global == popped.Type);
                    context.WriteFull($"global[{Index}] = {popped}");
                }
            }
        }

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