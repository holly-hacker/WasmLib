using System.IO;

namespace WasmLib.Utils
{
    internal interface IDeserializable
    {
        void Read(BinaryReader br);
    }
}
