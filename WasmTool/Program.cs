using System;
using System.Diagnostics;
using System.IO;
using EntryPoint;
using WasmLib;
using WasmLib.Decompilation;
using WasmTool.CommandLine;

namespace WasmTool
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            // print debug messages to console
            Trace.Listeners.Add(new ConsoleTraceListener());

            var arguments = Cli.Parse<CliArguments>(args);

            if (!File.Exists(arguments.InputFile)) {
                Console.WriteLine("Pass me a file as argument");
                return;
            }

            var sw = Stopwatch.StartNew();
            var wasmFile = WasmModule.Read(arguments.InputFile);
            sw.Stop();
            Console.WriteLine($"Read in {sw.Elapsed}");

            Console.WriteLine("wasm version: " + wasmFile.Version);

            sw = Stopwatch.StartNew();
            Stream outputStream = arguments.OutputFile is null
                ? Console.OpenStandardOutput()
                : File.Open(arguments.OutputFile, FileMode.Create);
            using var w = new StreamWriter(outputStream);
            IDecompiler dec = arguments.Decompiler switch {
                DecompilerKind.Disassembler => (IDecompiler)new DisassemblingDecompiler(wasmFile),
                DecompilerKind.IntermediateRepresentation => new IntermediateRepresentationDecompiler(wasmFile),
                _ => throw new Exception("Invalid decompiler type specified"),
            };

            for (int i = arguments.Skip; i < Math.Min(arguments.Skip + arguments.Count, wasmFile.FunctionBodies.Length); i++) {
                Debug.WriteLine($"Decompiling function {i} (0x{i:X})");
                dec.DecompileFunction(w, i);
            }

            sw.Stop();
            Console.WriteLine($"Written to {arguments.OutputFile} in {sw.Elapsed}");
        }
    }
}
