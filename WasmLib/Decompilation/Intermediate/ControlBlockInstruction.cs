using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class ControlBlockInstruction : IntermediateInstruction
    {
        public ControlBlockKind Kind { get; }
        public ValueKind ValueKind { get; }
        public IReadOnlyList<IntermediateInstruction> Block1 { get; }
        public IReadOnlyList<IntermediateInstruction>? Block2 { get; }
        
        public ControlBlockInstruction(in Instruction instruction, IReadOnlyList<IntermediateInstruction> block1, IReadOnlyList<IntermediateInstruction>? block2)
        {
            Kind = instruction.OpCode switch {
                OpCode.Block => ControlBlockKind.Block,
                OpCode.Loop => ControlBlockKind.Loop,
                OpCode.If => ControlBlockKind.If,
                _ => throw new WrongInstructionPassedException(instruction, nameof(ControlBlockInstruction))
            };

            ValueKind = (ValueKind)instruction.UIntOperand;
            
            Block1 = block1;
            Block2 = block2;
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            bool hasReturn = ValueKind != ValueKind.Empty;

            string keyword = EnumUtils.GetDescription(Kind);

            if (Kind == ControlBlockKind.If) {
                var popped = context.Pop();
                keyword += " " + popped;
            }
            
            if (hasReturn) {
                var pushed = context.Push(ValueKind);
                context.WriteFull($"{pushed} = {keyword} {{");
            }
            else {
                context.WriteFull($"{keyword} {{");
            }
            
            HandleBlock(ref context, Block1, hasReturn);

            if (Block2 != null) {
                Debug.Assert(Kind == ControlBlockKind.If);

                context.WriteFull("} else {");
                HandleBlock(ref context, Block2, hasReturn);
            }
            
            context.WriteFull("}");

            void HandleBlock(ref IntermediateContext context2, IReadOnlyList<IntermediateInstruction> block, bool hasReturn2)
            {
                context2.EnterBlock();
            
                foreach (IntermediateInstruction instruction in block) {
                    instruction.Handle(ref context2);
                }

                // if stack has values left on it, and we expect a return value
                if (hasReturn2 && !context2.RestOfBlockUnreachable) {
                    Debug.Assert(context2.StackIndices.Peek() != context2.Stack.Count);
                    
                    var popped = context2.Pop();
                    Debug.Assert(popped.Type == ValueKind);
                    context2.WriteFull($"block_return {popped}");
                }

                context2.ExitBlock();
            }
        }

        public enum ControlBlockKind
        {
            [Description("block")] Block,
            [Description("loop")] Loop,
            [Description("if")] If,
        }
    }
}