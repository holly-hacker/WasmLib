using System.Diagnostics.CodeAnalysis;
using EntryPoint;

namespace WasmTool.CommandLine
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CliArguments : BaseCliArguments
    {
        private const string Name = "WasmTool";
        
        public CliArguments() : base(Name) { }

        [Required]
        [OptionParameter("input", 'i')]
        public string InputFile { get; set; } = "";
        
        [OptionParameter("output", 'o')]
        public string? OutputFile { get; set; }
        
        [OptionParameter("decompiler", 'd')]
        public DecompilerKind Decompiler { get; set; }
        
        [OptionParameter("skip", 's')]
        public uint Skip { get; set; }

        [OptionParameter("count", 'n')]
        public uint Count { get; set; } = (uint)int.MaxValue;
    }
}