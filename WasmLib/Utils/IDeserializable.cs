using System.IO;

namespace WasmLib.Utils
{
    interface IDeserializable
    {
        void Read(BinaryReader br);
    }
}
