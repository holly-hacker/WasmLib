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
                OpCode.I32Load => (ActionKind.Load, ValueKind.I32, CastingKind.Same),
                OpCode.I64Load => (ActionKind.Load, ValueKind.I64, CastingKind.Same),
                OpCode.F32Load => (ActionKind.Load, ValueKind.F32, CastingKind.Same),
                OpCode.F64Load => (ActionKind.Load, ValueKind.F64, CastingKind.Same),
                OpCode.I32Load8S => (ActionKind.Load, ValueKind.I32, CastingKind.ByteSigned),
                OpCode.I32Load8U => (ActionKind.Load, ValueKind.I32, CastingKind.ByteUnsigned),
                OpCode.I32Load16S => (ActionKind.Load, ValueKind.I32, CastingKind.ShortSigned),
                OpCode.I32Load16U => (ActionKind.Load, ValueKind.I32, CastingKind.ShortUnsigned),
                OpCode.I64Load8S => (ActionKind.Load, ValueKind.I64, CastingKind.ByteSigned),
                OpCode.I64Load8U => (ActionKind.Load, ValueKind.I64, CastingKind.ByteUnsigned),
                OpCode.I64Load16S => (ActionKind.Load, ValueKind.I64, CastingKind.ShortSigned),
                OpCode.I64Load16U => (ActionKind.Load, ValueKind.I64, CastingKind.ShortUnsigned),
                OpCode.I64Load32S => (ActionKind.Load, ValueKind.I64, CastingKind.IntSigned),
                OpCode.I64Load32U => (ActionKind.Load, ValueKind.I64, CastingKind.IntUnsigned),
                OpCode.I32Store => (ActionKind.Store, ValueKind.I32, CastingKind.Same),
                OpCode.I64Store => (ActionKind.Store, ValueKind.I64, CastingKind.Same),
                OpCode.F32Store => (ActionKind.Store, ValueKind.F32, CastingKind.Same),
                OpCode.F64Store => (ActionKind.Store, ValueKind.F64, CastingKind.Same),
                OpCode.I32Store8 => (ActionKind.Store, ValueKind.I32, CastingKind.Byte),
                OpCode.I32Store16 => (ActionKind.Store, ValueKind.I32, CastingKind.Short),
                OpCode.I64Store8 => (ActionKind.Store, ValueKind.I64, CastingKind.Byte),
                OpCode.I64Store16 => (ActionKind.Store, ValueKind.I64, CastingKind.Short),
                OpCode.I64Store32 => (ActionKind.Store, ValueKind.I64, CastingKind.Int),
                _ => throw new WrongInstructionPassedException(instruction, nameof(MemoryInstruction))
            };
            
            // TODO: this logic/responsibility should prob be moved inside Instruction
            var operand = instruction.ULongOperand;
            Offset = (uint)(operand & 0xFFFFFFFF);
            Alignment = (uint)((operand & 0xFFFFFFFF00000000) >> 32);
        }

        public override void Handle(ref IntermediateContext context)
        {
            // TODO: Casting
            if (Action == ActionKind.Load && Casting != CastingKind.Same) {
                context.WriteFull($"// DECOMPILER WARNING: casting of type {Casting}");
            }
            
            Variable param = default;

            if (Action == ActionKind.Store) {
                param = context.Pop();
                Debug.Assert(param.Type == Type);
            }

            var popped = context.Pop();
            Debug.Assert(popped.Type == ValueKind.I32);

            if (Action == ActionKind.Load) {
                param = context.Push(Type);
            }

            string dereference = $"*({EnumUtils.GetDescription(Type)}*)({popped} + 0x{Offset:X})"; // NOTE: could be optimized
            
            context.WriteFull($"{(Action == ActionKind.Load ? $"{param} = {dereference}" : $"{dereference} = {param}")} // Alignment: 0x{1 << (int)Alignment:X}");
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