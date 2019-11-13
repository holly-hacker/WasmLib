using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class BranchInstruction : IntermediateInstruction
    {
        public BranchKind Kind { get; }
        public int Label { get; }
        public int[]? Labels { get; }
        
        public BranchInstruction(in Instruction instruction)
        {
            Kind = instruction.OpCode switch {
                OpCode.Br => BranchKind.Normal,
                OpCode.BrIf => BranchKind.Conditional,
                OpCode.BrTable => BranchKind.Table,
                _ => throw new WrongInstructionPassedException(instruction, nameof(BranchInstruction)),
            };

            Label = instruction.IntOperand;

            if (Kind == BranchKind.Table) {
                Labels = instruction.IntArrayOperand;
            }
        }
        
        public override void Handle(ref IntermediateContext context)
        {
            switch (Kind) {
                case BranchKind.Normal:
                    context.WriteFull($"BRANCH {Label}");
                    context.RestOfBlockUnreachable = true;
                    break;
                case BranchKind.Conditional:
                    var condition = context.Pop();
                    Debug.Assert(condition.Type == ValueKind.I32);
                    context.WriteFull($"BRANCH_IF({condition}) {Label}");
                    break;
                case BranchKind.Table:
                    Debug.Assert(Labels != null);
                    var index = context.Pop();
                    Debug.Assert(index.Type == ValueKind.I32);
                    context.WriteFull($"BRANCH_TABLE {{{string.Join(", ", Labels)}}}[{index}] ?? {Label}");
                    context.RestOfBlockUnreachable = true;
                    break;
            }
        }

        public enum BranchKind
        {
            Normal,
            Conditional,
            Table,
        }
    }
}