using System;
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

            var wasmFile = WasmFile.Read(filename);

            Console.WriteLine("wasm version: " + wasmFile.Version);
        }
    }
}
