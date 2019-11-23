using Rivers;

namespace WasmLib.Decompilation.SourceCode
{
    public interface IHasBlocks
    {
        Graph? Block1 { get; }
        Graph? Block2 { get; }
    }
}