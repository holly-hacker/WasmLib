using System.Diagnostics;
using WasmLib.FileFormat;

namespace WasmLib.Decompilation.Intermediate
{
    public class SelectInstruction : IntermediateInstruction
    {
        public override void Handle(ref IntermediateContext context)
        {
            var c = context.Pop();
            Debug.Assert(c.Type == ValueKind.I32, "Popped value was not i32");

            var val2 = context.Pop();
            var val1 = context.Pop();
            Debug.Assert(val1.Type == val2.Type, "val1 and val2 have different value types");

            var push = context.Push(val1.Type);
            context.WriteFull($"{push} = {c} ? {val1} : {val2}");
        }
    }
}