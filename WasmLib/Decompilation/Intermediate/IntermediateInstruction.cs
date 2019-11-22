using System.Diagnostics;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public abstract class IntermediateInstruction
    {
        // TODO: abstract pop/pushtypes private for caching
        public abstract ValueKind[] PopTypes { get; }
        public abstract ValueKind[] PushTypes { get; }
        public virtual bool RestOfBlockUnreachable => false;
        public virtual bool IsPure => false;
        public virtual bool IsImplicit => false;

        public bool HasBlock => Block1 != null;
        
        public ControlBlock? Block1 { get; protected set; }
        public ControlBlock? Block2 { get; protected set; }
        
        public int PopCount => PopTypes.Length;
        public int PushCount => PushTypes.Length;
        
        protected abstract string OperationStringFormat { get; }

        public void Handle(ref IntermediateContext context)
        {
            if (context.RestOfBlockUnreachable && IsImplicit) {
                #if DEBUG
                context.WriteFull("// omitted implicit instruction because rest of block is unreachable");
                #endif
                return;
            }

            var args = new Variable[PopCount];
            
            for (int i = 0; i < PopCount; i++) {
                args[i] = context.Pop();
                Debug.Assert(PopTypes[i] == args[i].Type || PopTypes[i] == ValueKind.Any || args[i].Type == ValueKind.Any);
            }
            
            context.RestOfBlockUnreachable = RestOfBlockUnreachable;
            
            // NOTE: really ugly and slow, but can't be replaced with string.format since input is dynamic and can contain {}
            string s = OperationStringFormat;
            for (int j = 0; j < args.Length; j++) {
                s = s.Replace($"{{{j}}}", args[j].ToString());
            }
            
            Debug.Assert(PushCount <= 1);
            if (PushCount > 0) {
                s = $"{context.Push(PushTypes[0])} = {s}";
            }

            if (HasBlock) {
                s += " {";
            }
            
            context.WriteFull(s);

            if (HasBlock) {
                HandleBlock(ref context, Block1);

                if (Block2 != null) {
                    context.WriteFull("} else {");
                    HandleBlock(ref context, Block2);
                }

                context.WriteFull("}");
            }

            static void HandleBlock(ref IntermediateContext context2, in ControlBlock block)
            {
                context2.EnterBlock();
            
                foreach (IntermediateInstruction instruction in block.Instructions) {
                    instruction.Handle(ref context2);
                }

                // if stack has values left on it, and we expect a return value
                if (block.HasReturn && !context2.RestOfBlockUnreachable) {
                    Debug.Assert(context2.StackIndices.Peek() != context2.Stack.Count);
                    
                    var popped = context2.Pop();
                    Debug.Assert(popped.Type == block.ValueKind);
                    context2.WriteFull($"block_return {popped}");
                }

                context2.ExitBlock();
            }
        }

        public override string ToString() => GetType().Name.Replace("Instruction", string.Empty);
    }
}