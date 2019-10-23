namespace WasmLib.FileFormat.Instructions
{
    public partial struct Instruction
    {
        public InstructionKind Opcode { get; private set; }
        private ulong operand;
        private object? operandObject;

        public Instruction(InstructionKind opcode)
        {
            Opcode = opcode;
            operand = 0;
            operandObject = null;
        }

        public Instruction(InstructionKind opcode, ulong operand)
        {
            Opcode = opcode;
            this.operand = operand;
            operandObject = null;
        }

        public Instruction(InstructionKind opcode, object operandObject)
        {
            Opcode = opcode;
            operand = 0;
            this.operandObject = operandObject;
        }

        public Instruction(InstructionKind opcode, ulong operand, object operandObject)
        {
            Opcode = opcode;
            this.operand = operand;
            this.operandObject = operandObject;
        }
    }
}
