using System.IO;
using WasmLib.Utils;

namespace WasmLib.FileFormat.Sections
{
    public class ElementSection
    {
        public Element[] Elements { get; private set; } = new Element[0];

        public static ElementSection Empty => new ElementSection();

        internal static ElementSection Read(BinaryReader reader)
        {
            return new ElementSection {
                Elements = reader.ReadWasmArray<Element>()
            };
        }
    }
}
