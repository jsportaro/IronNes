using System;
using System.Collections.Generic;
using System.Text;

namespace IronNes
{
    public class Operation
    {
        public string Mnemonic { get; set; }

        public byte Opcode { get; set; }

        public byte Cycles { get; set; }

        public Action Action { get; set; }
    }
}
