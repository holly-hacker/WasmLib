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
        
        public ControlBlockInstruction(in Instruction instruction, IReadOnlyList<IntermediateInstruction> block1, IReadOnlyList<IntermediateInstruction>? block2)
        {
            Kind = instruction.OpCode switch {
                OpCode.Block => ControlBlockKind.Block,
                OpCode.Loop => ControlBlockKind.Loop,
                OpCode.If => ControlBlockKind.If,
                _ => throw new WrongInstructionPassedException(instruction, nameof(ControlBlockInstruction))
            };

            ValueKind = (ValueKind)instruction.UIntOperand;
            
            Block1 = new ControlBlock(block1, ValueKind);
            
            if (block2 != null) {
                Block2 = new ControlBlock(block2, ValueKind);
            }
        }

        public override ValueKind[] PopTypes => Kind == ControlBlockKind.If ? new[] {ValueKind.I32} : new ValueKind[0];
        public override ValueKind[] PushTypes => ValueKind != ValueKind.Empty ? new[] {ValueKind} : new ValueKind[0];

        protected override string OperationStringFormat {
            get {
                string keyword = EnumUtils.GetDescription(Kind);

                if (Kind == ControlBlockKind.If) {
                    keyword += $" {{{(Block1!.HasReturn ? 1 : 0)}}}";
                }
                
                return (Block1!.HasReturn ? "{0} = " : string.Empty) + keyword + " {";
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