using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WasmLib.FileFormat.Instructions;
using WasmLib.Utils;

namespace WasmLib.FileFormat
{
    public class FunctionBody : IDeserializable
    {
        public ValueKind[] Locals => locals ?? throw new UninitializedFieldException();
        public Instruction[] Body => body ?? throw new UninitializedFieldException();

        private ValueKind[]? locals;
        private Instruction[]? body;

        public void Read(BinaryReader br)
        {
            uint bodySize = br.ReadVarUint32();
            var oldPos = br.BaseStream.Position;

            // read local declarations
            var localList = new List<ValueKind>();
            uint declarationCount = br.ReadVarUint32();
            for (int i = 0; i < declarationCount; i++) {
                var repeat = br.ReadVarUint32();
                var valKind = (ValueKind)br.ReadVarUint7();
                localList.AddRange(Enumerable.Repeat(valKind, (int)repeat));
            }

            locals = localList.ToArray();

            // read remaining bytes as function body
            body = Disassembler.Disassemble(br, (uint)(bodySize - (br.BaseStream.Position - oldPos))).ToArray();

            Debug.Assert(br.BaseStream.Position == oldPos + bodySize);
        }
    }
}
