using System;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate.Instructions
{
    public class BranchInstruction : IntermediateInstruction
    {
        public BranchKind Kind { get; }
        public uint Label { get; }
        public uint[]? Labels { get; }
        public override bool RestOfBlockUnreachable => Kind == BranchKind.Normal || Kind == BranchKind.Table;
        public override bool ModifiesControlFlow => true;

        public BranchInstruction(in Instruction instruction)
        {
            Kind = instruction.OpCode switch {
                OpCode.Br => BranchKind.Normal,
                OpCode.BrIf => BranchKind.Conditional,
                OpCode.BrTable => BranchKind.Table,
                _ => throw new WrongInstructionPassedException(instruction, nameof(BranchInstruction)),
            };

            Label = instruction.UIntOperand;

            if (Kind == BranchKind.Table) {
                Labels = instruction.UIntArrayOperand;
            }
        }

        public override ValueKind[] PopTypes => Kind == BranchKind.Conditional || Kind == BranchKind.Table ? new[] {ValueKind.I32} : new ValueKind[0];
        public override ValueKind[] PushTypes => new ValueKind[0];

        public override string OperationStringFormat => Kind switch {
            BranchKind.Normal => $"BRANCH {Label}",
            BranchKind.Conditional => $"BRANCH_IF({{0}}) {Label}",
            BranchKind.Table => $"BRANCH_TABLE {{{string.Join(", ", Labels ?? throw new Exception("Found no labels for br_table instruction"))}}}[{{0}}] ?? {Label}",
            _ => throw new IndexOutOfRangeException()
        };

        public override string ToString() => $"Branch {Kind}";

        public enum BranchKind
        {
            Normal,
            Conditional,
            Table,
        }
    }
}