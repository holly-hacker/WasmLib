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
        
        public ControlBlockInstruction(Instruction instruction, IReadOnlyList<IntermediateInstruction> block1, IReadOnlyList<IntermediateInstruction>? block2)
        {
            Kind = instruction.OpCode switch {
                InstructionKind.Block => ControlBlockKind.Block,
                InstructionKind.Loop => ControlBlockKind.Loop,
                InstructionKind.If => ControlBlockKind.If,
                _ => throw new WrongInstructionPassedException(instruction, nameof(ControlBlockInstruction))
            };

            ValueKind = (ValueKind)instruction.UIntOperand;
            
            Block1 = block1;
            Block2 = block2;
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            bool hasReturn = ValueKind != ValueKind.Empty;

            if (hasReturn) {
                Debug.Assert(Kind == ControlBlockKind.If);
            }

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

            if (Kind == ControlBlockKind.If && Block2 != null) {
                context.WriteFull("} else {");
                HandleBlock(ref context, Block2, hasReturn);
            }
            
            context.WriteFull("}");

            void HandleBlock(ref IntermediateContext contextPassed, IReadOnlyList<IntermediateInstruction> block, bool hasReturnPassed)
            {
                contextPassed.Indent();
            
                foreach (IntermediateInstruction instruction in block) {
                    instruction.Handle(ref contextPassed);
                }

                if (hasReturnPassed) {
                    var popped = contextPassed.Pop();
                    Debug.Assert(popped.Type == ValueKind);
                    contextPassed.WriteFull($"if_return {popped}");
                }
            
                contextPassed.DeIndent();
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