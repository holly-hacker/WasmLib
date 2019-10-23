namespace WasmLib.FileFormat.Instructions
{
    public enum OperandKind
    {
        None,
        /// <remarks> https://webassembly.github.io/spec/core/binary/types.html#binary-blocktype </remarks>
        BlockType,
        /// <summary>
        /// Refers to nested control flow constructs, with 0 being the current "block"
        /// </summary>
        LabelIndex,
        /// <remarks> for br_table </remarks>
        BrTableOperand,
        FuncIndex,
        /// <remarks> for call_indirect, has a 0x00 after it in case the call target is invalid </remarks>
        IndirectCallTypeIndex,
        LocalIndex,
        GlobalIndex,
        /// <summary> u32 offset + u32 align </summary>
        MemArg,
        /// <summary> A single 0x00 byte </summary>
        /// <remarks> for memory.size and memory.grow </remarks>
        Zero,
        ImmediateI32,
        ImmediateI64,
        ImmediateF32,
        ImmediateF64,
    }
}
