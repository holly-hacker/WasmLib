using System;
using System.Diagnostics;
using System.IO;
using WasmLib;
using WasmLib.Decompilation;

namespace WasmTool
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Pass me an argument");
                return;
            }

            string filename = args[0];

            if (!File.Exists(filename)) {
                Console.WriteLine("Pass me a file as argument");
                return;
            }

            var sw = Stopwatch.StartNew();
            var wasmFile = WasmFile.Read(filename);
            sw.Stop();
            Console.WriteLine($"Read in {sw.Elapsed}");

            Console.WriteLine("wasm version: " + wasmFile.Version);

            sw = Stopwatch.StartNew();
            using var fs = File.Open("out.txt", FileMode.Create);
            using var w = new StreamWriter(fs);
            IDecompiler dec = new IntermediateRepresentationDecompiler(wasmFile);

            for (int i = 0; i < Math.Min(wasmFile.FunctionBodies.Length, 67); i++) {
                dec.DecompileFunction(w, i);
                Console.WriteLine($"Decompiled function {i} (0x{i:X})");
            }

            sw.Stop();
            Console.WriteLine($"Written to out.txt in {sw.Elapsed}");
        }
    }
}
