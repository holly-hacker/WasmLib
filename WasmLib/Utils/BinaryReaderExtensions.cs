using System;
using System.IO;
using System.Linq;
using System.Text;
using WasmLib.FileFormat;
using WasmLib.FileFormat.Instructions;

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

        public static ulong ReadVarUint64(this BinaryReader br)
        {
            ulong ret = 0;

            for (int i = 0; i < 10; i++) {
                byte read = br.ReadByte();
                ret |= (ulong)((read & 0x7f) << (i * 7));

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

        public static Instruction[] ReadExpression(this BinaryReader br) => Disassembler.DisassembleExpression(br).ToArray();
    }
}
