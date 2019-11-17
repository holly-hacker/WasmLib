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
        
        public ControlBlock? Block1 { get; protected set; }
        public ControlBlock? Block2 { get; protected set; }
        
        public int PopCount => PopTypes.Length;
        public int PushCount => PushTypes.Length;
        
        protected abstract string OperationStringFormat { get; }

        public void Handle(ref IntermediateContext context)
        {
            var args = new Variable[PushCount + PopCount];
            
            int i;
            for (i = PushCount; i < PushCount + PopCount; i++) {
                args[i] = context.Pop();
                Debug.Assert(PopTypes[i - PushCount] == args[i].Type || PopTypes[i - PushCount] == ValueKind.Any || args[i].Type == ValueKind.Any);
            }
            for (i = 0; i < PushCount; i++) {
                args[i] = context.Push(PushTypes[i]);
            }

            context.RestOfBlockUnreachable = RestOfBlockUnreachable;
            
            // NOTE: really ugly and slow
            string s = OperationStringFormat;
            for (int j = 0; j < args.Length; j++) {
                s = s.Replace($"{{{j}}}", args[j].ToString());
            }
            
            context.WriteFull(s);

            if (Block1 != null) {
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
    }
}