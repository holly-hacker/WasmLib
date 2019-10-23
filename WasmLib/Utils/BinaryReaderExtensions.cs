using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WasmLib.FileFormat;

namespace WasmLib.Utils
{
    static class BinaryReaderExtensions
    {
        public static byte ReadVarUint7(this BinaryReader br)
        {
            byte read = br.ReadByte();

            if ((read & 0x80) != 0) {
                throw new Exception("Exceeded maximum length for VarUint7");
            }

            return (byte)(read & 0x7f);
        }

        public static uint ReadVarUint32(this BinaryReader br)
        {
            uint ret = 0;

            for (int i = 0; i < 5; i++) {
                byte read = br.ReadByte();
                ret |= (uint)((read & 0x7f) << (i * 7));

                if ((read & 0x80) == 0) {
                    return ret;
                }
            }

            throw new Exception("Exceeded maximum length for VarUint32");
        }

        public static int ReadVarInt32(this BinaryReader br) => (int)br.ReadVarUint32();

        public static T[] ReadWasmArray<T>(this BinaryReader br) where T : IDeserializable, new()
        {
            var arr = new T[br.ReadVarInt32()];

            for (int i = 0; i < arr.Length; i++) {
                arr[i] = new T();
                arr[i].Read(br);
            }

            return arr;
        }

        public static byte[] ReadWasmByteArray(this BinaryReader br) => br.ReadBytes(br.ReadVarInt32());
        public static ValueKind[] ReadValueKindArray(this BinaryReader br) => br.ReadBytes(br.ReadVarInt32()).Cast<ValueKind>().ToArray();

        public static uint[] ReadVarUint32Array(this BinaryReader br)
        {
            var arr = new uint[br.ReadVarInt32()];

            for (int i = 0; i < arr.Length; i++) {
                arr[i] = br.ReadVarUint32();
            }

            return arr;
        }

        public static string ReadIdentifier(this BinaryReader br) => Encoding.UTF8.GetString(br.ReadWasmByteArray());

        public static byte[] ReadExpression(this BinaryReader br)
        {
            // TODO: properly disassemble
            // for now, just read until the next 0x0B byte, which represents the END opcode. This is not perfect as other instructions may contain it.
            var list = new List<byte>();
            byte b;
            do {
                b = br.ReadByte();
                list.Add(b);
            } while (b != 0x0B);

            return list.ToArray();
        }
    }
}
