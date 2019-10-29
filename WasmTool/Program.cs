using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;

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

            int index = 0;
            foreach (FunctionBody body in wasmFile.FunctionBodies.Take(20)) {
                Console.WriteLine($"fun_{index++:X8}:");
                foreach (Instruction instruction in body.Body) {
                    Console.WriteLine("\t" + instruction);
                }

                Console.WriteLine();
            }

            Console.WriteLine("wasm version: " + wasmFile.Version);
        }
    }
}
