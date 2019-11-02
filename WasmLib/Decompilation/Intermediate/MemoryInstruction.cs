using System.ComponentModel;
using System.Diagnostics;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.Decompilation.Intermediate
{
    public class MemoryInstruction : IntermediateInstruction
    {
        public ActionKind Action { get; }
        public ValueKind Type { get; }
        public CastingKind Casting { get; }

        public uint Offset { get; }
        public uint Alignment { get; }
        
        public MemoryInstruction(Instruction instruction)
        {
            (Action, Type, Casting) = instruction.OpCode switch {
                InstructionKind.I32Load => (ActionKind.Load, ValueKind.I32, CastingKind.Same),
                InstructionKind.I64Load => (ActionKind.Load, ValueKind.I64, CastingKind.Same),
                InstructionKind.F32Load => (ActionKind.Load, ValueKind.F32, CastingKind.Same),
                InstructionKind.F64Load => (ActionKind.Load, ValueKind.F64, CastingKind.Same),
                InstructionKind.I32Load8S => (ActionKind.Load, ValueKind.I32, CastingKind.ByteSigned),
                InstructionKind.I32Load8U => (ActionKind.Load, ValueKind.I32, CastingKind.ByteUnsigned),
                InstructionKind.I32Load16S => (ActionKind.Load, ValueKind.I32, CastingKind.ShortSigned),
                InstructionKind.I32Load16U => (ActionKind.Load, ValueKind.I32, CastingKind.ShortUnsigned),
                InstructionKind.I64Load8S => (ActionKind.Load, ValueKind.I64, CastingKind.ByteSigned),
                InstructionKind.I64Load8U => (ActionKind.Load, ValueKind.I64, CastingKind.ByteUnsigned),
                InstructionKind.I64Load16S => (ActionKind.Load, ValueKind.I64, CastingKind.ShortSigned),
                InstructionKind.I64Load16U => (ActionKind.Load, ValueKind.I64, CastingKind.ShortUnsigned),
                InstructionKind.I64Load32S => (ActionKind.Load, ValueKind.I64, CastingKind.IntSigned),
                InstructionKind.I64Load32U => (ActionKind.Load, ValueKind.I64, CastingKind.IntUnsigned),
                InstructionKind.I32Store => (ActionKind.Store, ValueKind.I32, CastingKind.Same),
                InstructionKind.I64Store => (ActionKind.Store, ValueKind.I64, CastingKind.Same),
                InstructionKind.F32Store => (ActionKind.Store, ValueKind.F32, CastingKind.Same),
                InstructionKind.F64Store => (ActionKind.Store, ValueKind.F64, CastingKind.Same),
                InstructionKind.I32Store8 => (ActionKind.Store, ValueKind.I32, CastingKind.Byte),
                InstructionKind.I32Store16 => (ActionKind.Store, ValueKind.I32, CastingKind.Short),
                InstructionKind.I64Store8 => (ActionKind.Store, ValueKind.I64, CastingKind.Byte),
                InstructionKind.I64Store16 => (ActionKind.Store, ValueKind.I64, CastingKind.Short),
                InstructionKind.I64Store32 => (ActionKind.Store, ValueKind.I64, CastingKind.Int),
                _ => throw new WrongInstructionPassedException(instruction, nameof(MemoryInstruction))
            };
            
            // TODO: this logic/responsibility should prob be moved inside Instruction
            var operand = instruction.ULongOperand;
            Offset = (uint)(operand & 0xFFFFFFFF);
            Alignment = (uint)((operand & 0xFFFFFFFF00000000) >> 32);
        }

        public override void Handle(ref IntermediateContext context)
        {
            var param = Action == ActionKind.Load ? context.Push(Type) : context.Pop();
            Debug.Assert(param.Type == Type);

            string typeString = EnumUtils.GetDescription(Type);

            string deref = $"*({typeString}*)0x{Offset:x8}";
            
            // TODO: Casting
            if (Action == ActionKind.Load && Casting != CastingKind.Same) {
                context.WriteFull($"// DECOMPILER WARNING: casting of type {Casting}");
            }
            
            context.WriteFull(Action == ActionKind.Load ? $"{param} = {deref}" : $"{deref} = {param}");
        }

        public enum ActionKind
        {
            [Description("load")] Load,
            [Description("store")] Store,
        }

        public enum CastingKind
        {
            Same,
            Byte,
            ByteSigned,
            ByteUnsigned,
            Short,
            ShortSigned,
            ShortUnsigned,
            Int,
            IntSigned,
            IntUnsigned,
        }
    }
}