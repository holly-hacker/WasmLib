using System.Diagnostics;

namespace WasmLib.Decompilation.Intermediate
{
    /// <remarks>
    /// This is a pseudo-instruction to handle implicit returns at the end of a function
    /// </remarks>
    public class FakeReturnInstruction : IntermediateInstruction
    {
        public override void Handle(ref IntermediateContext context)
        {
            if (context.Stack.Count != 0) {
                Debug.Assert(context.Signature.ReturnParameter.Length == 1, "Tried to create fake return for function that returns void");
                var popped = context.Pop();
                Debug.Assert(popped.Type == context.Signature.ReturnParameter[0], "Mismatch between return type and stack");
                context.WriteFull($"return {popped}");
            }
            else {
                Debug.Assert(context.Signature.ReturnParameter.Length == 0, "No value on stack for function that doesn't return void");
            }
        }
    }
}