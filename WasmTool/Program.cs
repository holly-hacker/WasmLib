using System;
using System.Diagnostics;
using System.IO;
using WasmLib;

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
        }
    }
}
