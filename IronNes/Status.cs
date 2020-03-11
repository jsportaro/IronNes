using System;
using System.Collections.Generic;
using System.Text;

namespace IronNes
{
    [Flags]
    public enum Status : byte
    {
        C = 1,   // Carry 
        Z = 2,   // Zero
        I = 4,   // Interrupt (IRQ Disable)
        D = 8,   // Decimal 
        B = 16,  // Break
        X = 32,  // Ignored
        V = 64,  // Overflow
        N = 128  // Negative
    }
}
