using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            if (ValueKind != ValueKind.Empty) {
                throw new NotImplementedException();
            }

            string keyword = EnumUtils.GetDescription(Kind);

            if (Kind == ControlBlockKind.If) {
                var popped = context.Pop();
                keyword += " " + popped;
            }
            
            context.WriteFull($"{keyword} {{");
            context.Indent();
            foreach (IntermediateInstruction instruction in Block1) {
                instruction.Handle(ref context);
            }
            context.DeIndent();

            if (Kind == ControlBlockKind.If && Block2 != null) {
                context.WriteFull("} else {");
                
                context.Indent();
                foreach (IntermediateInstruction instruction in Block2) {
                    instruction.Handle(ref context);
                }
                context.DeIndent();
            }
            context.WriteFull("}");
        }

        public enum ControlBlockKind
        {
            [Description("block")] Block,
            [Description("loop")] Loop,
            [Description("if")] If,
        }
    }
}