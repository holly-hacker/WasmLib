using System.Diagnostics;

namespace WasmLib.Decompilation.Intermediate
{
    public class ReturnInstruction : IntermediateInstruction
    {
        public override void Handle(ref IntermediateContext context)
        {
            if (context.Signature.ReturnParameter.Length > 0) {
                var popped = context.Pop();
                Debug.Assert(popped.Type == context.Signature.ReturnParameter[0], "Mismatch between return type and stack");
                context.WriteFull($"return {popped}");
            }
            else {
                context.WriteFull("return");
            }
        }
    }
}