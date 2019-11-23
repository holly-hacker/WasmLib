using System.Collections.Generic;
using System.ComponentModel;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class ControlBlockInstruction : IntermediateInstruction
    {
        public ControlBlockKind Kind { get; }
        public ValueKind ValueKind { get; }
        public override bool IsOrderImportant => true; // TODO: could be optimized by checking if control blocks are pure
        public override bool CanBeInlined => false;
        
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

        public override string OperationStringFormat {
            get {
                string keyword = EnumUtils.GetDescription(Kind);

                if (Kind == ControlBlockKind.If) {
                    keyword += " {0}";
                }

                return keyword;
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