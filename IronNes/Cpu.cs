using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IronNes
{
    public class Cpu : IClockable
    {
        private Memory memory;

        public byte A    { get; private set; }
        public byte X    { get; private set; }
        public byte Y    { get; private set; }
        public byte S    { get; private set; }
        public ushort PC { get; private set; }

        // Status Register
        public Status   SR { get; private set; }

        private Operation[] Instructions;

        public void Reset()
        {
            PC = memory.ReadDouble(0xfffc);

            SR &= 0;
        }

        public Cpu(Memory memory)
        {
            this.memory = memory;

            Reset();

            Instructions = new Operation[]
            {
                    new Operation { Mnemonic = "BRK", Cycles = 7, Action = BRKIMP  },  // 0x00
                    new Operation { Mnemonic = "ORA", Action = ORAXIND },  // 0x01
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x02
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x03
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x04
                    new Operation { Mnemonic = "ORA", Action = ORAZPG  },  // 0x05
                    new Operation { Mnemonic = "ASL", Action = ASLZPG  },  // 0x06
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x07
                    new Operation { Mnemonic = "PHP", Action = PHPIMP  },  // 0x08
                    new Operation { Mnemonic = "ORA", Action = ORAIMM  },  // 0x09
                    new Operation { Mnemonic = "ASL", Action = ASLA    },  // 0x0A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x0B
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x0C
                    new Operation { Mnemonic = "ORA", Action = ORAABS  },  // 0x0D
                    new Operation { Mnemonic = "ASL", Action = ASLABS  },  // 0x0E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x0F
                    new Operation { Mnemonic = "BPL", Action = BPLREL  },  // 0x10
                    new Operation { Mnemonic = "ORA", Action = ORAINDY },  // 0x11
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x12
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x13
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x14
                    new Operation { Mnemonic = "ORA", Action = ORAZPGX },  // 0x15
                    new Operation { Mnemonic = "ASL", Action = ASLZPGX },  // 0x16
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x17
                    new Operation { Mnemonic = "CLC", Action = CLCIMP  },  // 0x18
                    new Operation { Mnemonic = "ORA", Action = ORAABSY },  // 0x19
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x1A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x1B
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x1C
                    new Operation { Mnemonic = "ORA", Action = ORAABSX },  // 0x1D
                    new Operation { Mnemonic = "ASL", Action = ASLABSX },  // 0x1E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x1F
                    new Operation { Mnemonic = "JSR", Action = JSRABS  },  // 0x20
                    new Operation { Mnemonic = "AND", Action = ANDXIND },  // 0x21
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x22
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x23
                    new Operation { Mnemonic = "BIT", Action = BITZPG  },  // 0x24
                    new Operation { Mnemonic = "AND", Action = ANDZPG  },  // 0x25
                    new Operation { Mnemonic = "ROL", Action = ROLZPG  },  // 0x26
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x27
                    new Operation { Mnemonic = "PLP", Action = PLPIMP  },  // 0x28
                    new Operation { Mnemonic = "AND", Action = ANDIMM  },  // 0x29
                    new Operation { Mnemonic = "ROL", Action = ROLA    },  // 0x2A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x2B
                    new Operation { Mnemonic = "BIT", Action = BITABS, Cycles = 4  },  // 0x2C
                    new Operation { Mnemonic = "AND", Action = ANDABS  },  // 0x2D
                    new Operation { Mnemonic = "ROL", Action = ROLABS  },  // 0x2E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x2F
                    new Operation { Mnemonic = "BMI", Action = BMIREL  },  // 0x30
                    new Operation { Mnemonic = "AND", Action = ANDINDY },  // 0x31
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x32
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x33
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x34
                    new Operation { Mnemonic = "AND", Action = ANDZPG  },  // 0x35
                    new Operation { Mnemonic = "ROL", Action = ROLZPGX },  // 0x36
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x37
                    new Operation { Mnemonic = "SEC", Action = SECIMP  },  // 0x38
                    new Operation { Mnemonic = "AND", Action = ANDABSY },  // 0x39
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x3A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x3B
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x3C
                    new Operation { Mnemonic = "AND", Action = ANDABSX },  // 0x3D
                    new Operation { Mnemonic = "ROL", Action = ROLABSX },  // 0x3E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x3F
                    new Operation { Mnemonic = "RTI", Action = RTIIMP  },  // 0x40
                    new Operation { Mnemonic = "EOR", Action = EORXIND },  // 0x41
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x42
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x43
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x44
                    new Operation { Mnemonic = "EOR", Action = EORZPG  },  // 0x45
                    new Operation { Mnemonic = "LSR", Action = LSRZPG  },  // 0x46
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x47
                    new Operation { Mnemonic = "PHA", Action = PHAIMP  },  // 0x48
                    new Operation { Mnemonic = "EOR", Action = EORIM   },  // 0x49
                    new Operation { Mnemonic = "LSR", Action = LSRA    },  // 0x4A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x4B
                    new Operation { Mnemonic = "JMP", Action = JMPABS  },  // 0x4C
                    new Operation { Mnemonic = "EOR", Action = EORABS  },  // 0x4D
                    new Operation { Mnemonic = "LSR", Action = LSRABS  },  // 0x4E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x4F
                    new Operation { Mnemonic = "BVC", Action = BVCREL  },  // 0x50
                    new Operation { Mnemonic = "EOR", Action = EORINDY },  // 0x51
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x52
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x53
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x54
                    new Operation { Mnemonic = "EOR", Action = EORZPGX },  // 0x55
                    new Operation { Mnemonic = "LSR", Action = LSRZPGX },  // 0x56
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x57
                    new Operation { Mnemonic = "CLI", Action = CLIIMP  },  // 0x58
                    new Operation { Mnemonic = "EOR", Action = EORABSY },  // 0x59
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x5A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x5B
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x5C
                    new Operation { Mnemonic = "EOR", Action = EORABSX },  // 0x5D
                    new Operation { Mnemonic = "LSR", Action = LSRABSX },  // 0x5E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x5F
                    new Operation { Mnemonic = "RTS", Action = RTSIMP  },  // 0x60
                    new Operation { Mnemonic = "ADC", Action = ADCXIND },  // 0x61
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x62
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x63
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x64
                    new Operation { Mnemonic = "ADC", Action = ADCZPG  },  // 0x65
                    new Operation { Mnemonic = "ROR", Action = RORZPG  },  // 0x66
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x67
                    new Operation { Mnemonic = "PLA", Action = PLAIMP  },  // 0x68
                    new Operation { Mnemonic = "ADC", Action = ADCIMM  },  // 0x69
                    new Operation { Mnemonic = "ROR", Action = RORA    },  // 0x6A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x6B
                    new Operation { Mnemonic = "JMP", Action = JMPIND  },  // 0x6C
                    new Operation { Mnemonic = "ADC", Action = ADCABS  },  // 0x6D
                    new Operation { Mnemonic = "ROR", Action = RORABS  },  // 0x6E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x6F
                    new Operation { Mnemonic = "BVS", Action = BVSREL  },  // 0x70
                    new Operation { Mnemonic = "ADC", Action = ADCINDY },  // 0x71
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x72
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x73
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x74
                    new Operation { Mnemonic = "ADC", Action = ADCZPGX },  // 0x75
                    new Operation { Mnemonic = "ROR", Action = RORZPGX },  // 0x76
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x77
                    new Operation { Mnemonic = "SEI", Action = SEIIMP, Cycles = 2  },  // 0x78
                    new Operation { Mnemonic = "ADC", Action = ADCABSY },  // 0x79
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x7A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x7B
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x7C
                    new Operation { Mnemonic = "ADC", Action = ADCABSX },  // 0x7D
                    new Operation { Mnemonic = "ROR", Action = RORABSX },  // 0x7E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x7F
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x80
                    new Operation { Mnemonic = "STA", Action = STAXIND },  // 0x81
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x82
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x83
                    new Operation { Mnemonic = "STY", Action = STYZPG  },  // 0x84
                    new Operation { Mnemonic = "STA", Action = STAZPG  },  // 0x85
                    new Operation { Mnemonic = "STX", Action = STXZPG  },  // 0x86
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x87
                    new Operation { Mnemonic = "DEY", Action = DEYIMP  },  // 0x88
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x89
                    new Operation { Mnemonic = "TXA", Action = TXAIMP  },  // 0x8A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x8B
                    new Operation { Mnemonic = "STY", Action = STYABS  },  // 0x8C
                    new Operation { Mnemonic = "STA", Action = STAABS  },  // 0x8D
                    new Operation { Mnemonic = "STX", Action = STXABS, Cycles = 4  },  // 0x8E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x8F
                    new Operation { Mnemonic = "BCC", Action = BCCREL  },  // 0x90
                    new Operation { Mnemonic = "STA", Action = STAINDY },  // 0x91
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x92
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x93
                    new Operation { Mnemonic = "STY", Action = STYZPGX },  // 0x94
                    new Operation { Mnemonic = "STA", Action = STAZPGX },  // 0x95
                    new Operation { Mnemonic = "STX", Action = STXZPGY },  // 0x96
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x97
                    new Operation { Mnemonic = "TYA", Action = TYAIMP  },  // 0x98
                    new Operation { Mnemonic = "STA", Action = STAABSY },  // 0x99
                    new Operation { Mnemonic = "TXS", Action = TXSIMP  },  // 0x9A
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x9B
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x9C
                    new Operation { Mnemonic = "STA", Action = STAABSX },  // 0x9D
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x9E
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0x9F
                    new Operation { Mnemonic = "LDY", Action = LDYIMM  },  // 0xA0
                    new Operation { Mnemonic = "LDA", Action = LDAXIND },  // 0xA1
                    new Operation { Mnemonic = "LDX", Action = LDXIMM, Cycles = 2 },  // 0xA2
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xA3
                    new Operation { Mnemonic = "LDY", Action = LDYZPG  },  // 0xA4
                    new Operation { Mnemonic = "LDA", Action = LDAZPG  },  // 0xA5
                    new Operation { Mnemonic = "LDX", Action = LDXZPG  },  // 0xA6
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xA7
                    new Operation { Mnemonic = "TAY", Action = TAYIMP  },  // 0xA8
                    new Operation { Mnemonic = "LDA", Action = LDAIMM  },  // 0xA9
                    new Operation { Mnemonic = "TAX", Action = TAXIMP  },  // 0xAA
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xAB
                    new Operation { Mnemonic = "LDY", Action = LDYABS  },  // 0xAC
                    new Operation { Mnemonic = "LDA", Action = LDAABS  },  // 0xAD
                    new Operation { Mnemonic = "LDX", Action = LDXABS  },  // 0xAE
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xAF
                    new Operation { Mnemonic = "BCS", Action = BCSREL  },  // 0xB0
                    new Operation { Mnemonic = "LDA", Action = LDAINDY },  // 0xB1
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xB2
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xB3
                    new Operation { Mnemonic = "LDY", Action = LDYZPGY },  // 0xB4
                    new Operation { Mnemonic = "LDA", Action = LDAZPGX },  // 0xB5
                    new Operation { Mnemonic = "LDX", Action = LDXZPGY },  // 0xB6
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xB7
                    new Operation { Mnemonic = "CLV", Action = CLVIMP  },  // 0xB8
                    new Operation { Mnemonic = "LDA", Action = LDAABSY },  // 0xB9
                    new Operation { Mnemonic = "TSX", Action = TSXIMP  },  // 0xBA
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xBB
                    new Operation { Mnemonic = "LDY", Action = LDYABSX },  // 0xBC
                    new Operation { Mnemonic = "LDA", Action = LDAABSX },  // 0xBD
                    new Operation { Mnemonic = "LDX", Action = LDXABSY },  // 0xBE
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xBF
                    new Operation { Mnemonic = "CPY", Action = CPYIMM  },  // 0xC0
                    new Operation { Mnemonic = "CMP", Action = CMPXIND },  // 0xC1
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xC2
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xC3
                    new Operation { Mnemonic = "CPY", Action = CPYZPG  },  // 0xC4
                    new Operation { Mnemonic = "CMP", Action = ZMPZPG  },  // 0xC5
                    new Operation { Mnemonic = "DEC", Action = DECZPG  },  // 0xC6
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xC7
                    new Operation { Mnemonic = "INY", Action = INYIMP  },  // 0xC8
                    new Operation { Mnemonic = "CMP", Action = CMPIMM  },  // 0xC9
                    new Operation { Mnemonic = "DEX", Action = DEXIMP  },  // 0xCA
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xCB
                    new Operation { Mnemonic = "CPY", Action = CPYABS  },  // 0xCC
                    new Operation { Mnemonic = "CMP", Action = CMPABS  },  // 0xCD
                    new Operation { Mnemonic = "DEC", Action = DECABS  },  // 0xCE
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xCF
                    new Operation { Mnemonic = "BNE", Action = BNEREL  },  // 0xD0
                    new Operation { Mnemonic = "CMP", Action = CMPINDY },  // 0xD1
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xD2
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xD3
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xD4
                    new Operation { Mnemonic = "CMP", Action = CMPZPGX },  // 0xD5
                    new Operation { Mnemonic = "DEC", Action = DECZPGX },  // 0xD6
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xD7
                    new Operation { Mnemonic = "CLD", Action = CLDIMP, Cycles = 2  },  // 0xD8
                    new Operation { Mnemonic = "CMP", Action = CMPABSY },  // 0xD9
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xDA
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xDB
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xDC
                    new Operation { Mnemonic = "CMP", Action = CMPABSX },  // 0xDD
                    new Operation { Mnemonic = "DEC", Action = DECABSX },  // 0xDE
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xDF
                    new Operation { Mnemonic = "CPX", Action = CPXIMM  },  // 0xE0
                    new Operation { Mnemonic = "SBC", Action = SBCXIND },  // 0xE1
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xE2
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xE3
                    new Operation { Mnemonic = "CPX", Action = CPXZPG  },  // 0xE4
                    new Operation { Mnemonic = "SBC", Action = SBCZPG  },  // 0xE5
                    new Operation { Mnemonic = "INC", Action = INCZPG  },  // 0xE6
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xE7
                    new Operation { Mnemonic = "INX", Action = INXIMP  },  // 0xE8
                    new Operation { Mnemonic = "SBC", Action = SBCIMM  },  // 0xE9
                    new Operation { Mnemonic = "NOP", Action = NOP     },  // 0xEA
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xEB
                    new Operation { Mnemonic = "CPX", Action = CPXABS  },  // 0xEC
                    new Operation { Mnemonic = "SBC", Action = SBCABS  },  // 0xED
                    new Operation { Mnemonic = "INC", Action = INCABS  },  // 0xEE
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xEF
                    new Operation { Mnemonic = "BEQ", Action = BEQREL  },  // 0xF0
                    new Operation { Mnemonic = "SBC", Action = SBCXIND },  // 0xF1
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xF2
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xF3
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xF4
                    new Operation { Mnemonic = "SBC", Action = SBCZPGX },  // 0xF5
                    new Operation { Mnemonic = "INC", Action = INCZPGX },  // 0xF6
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xF7
                    new Operation { Mnemonic = "SED", Action = SEDIMP  },  // 0xF8
                    new Operation { Mnemonic = "SBC", Action = SBCABSY },  // 0xF9
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xFA
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xFB
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xFC
                    new Operation { Mnemonic = "SBC", Action = SBCABSX },  // 0xFD
                    new Operation { Mnemonic = "INC", Action = INCABSX },  // 0xFE
                    new Operation { Mnemonic = "FLT", Action = FLT     },  // 0xFF
            };
        }

        private void NOP()
        {
            throw new NotImplementedException();
        }

        private void SBCIMM()
        {
            throw new NotImplementedException();
        }

        private void INXIMP()
        {
            throw new NotImplementedException();
        }

        private void INCZPG()
        {
            throw new NotImplementedException();
        }

        private void SBCZPG()
        {
            throw new NotImplementedException();
        }

        private void CPXZPG()
        {
            throw new NotImplementedException();
        }

        private void CPXIMM()
        {
            throw new NotImplementedException();
        }

        private void DECABSX()
        {
            throw new NotImplementedException();
        }

        private void CMPABSX()
        {
            throw new NotImplementedException();
        }

        private void CMPABSY()
        {
            throw new NotImplementedException();
        }

        private void CLDIMP()
        {
            SR &= ~Status.D;
        }

        private void DECZPGX()
        {
            throw new NotImplementedException();
        }

        private void CMPZPGX()
        {
            throw new NotImplementedException();
        }

        private void CMPINDY()
        {
            throw new NotImplementedException();
        }

        private void BNEREL()
        {
            throw new NotImplementedException();
        }

        private void DECABS()
        {
            throw new NotImplementedException();
        }

        private void CMPABS()
        {
            throw new NotImplementedException();
        }

        private void CPYABS()
        {
            throw new NotImplementedException();
        }

        private void DEXIMP()
        {
            throw new NotImplementedException();
        }

        private void CMPIMM()
        {
            throw new NotImplementedException();
        }

        private void INYIMP()
        {
            throw new NotImplementedException();
        }

        private void DECZPG()
        {
            throw new NotImplementedException();
        }

        private void ZMPZPG()
        {
            throw new NotImplementedException();
        }

        private void CPYZPG()
        {
            throw new NotImplementedException();
        }

        private void CMPXIND()
        {
            throw new NotImplementedException();
        }

        private void CPYIMM()
        {
            throw new NotImplementedException();
        }

        private void LDXABSY()
        {
            throw new NotImplementedException();
        }

        private void LDAABSX()
        {
            throw new NotImplementedException();
        }

        private void LDYABSX()
        {
            throw new NotImplementedException();
        }

        private void TSXIMP()
        {
            throw new NotImplementedException();
        }

        private void LDAABSY()
        {
            throw new NotImplementedException();
        }

        private void CLVIMP()
        {
            throw new NotImplementedException();
        }

        private void LDXZPGY()
        {
            throw new NotImplementedException();
        }

        private void LDAZPGX()
        {
            throw new NotImplementedException();
        }

        private void LDYZPGY()
        {
            throw new NotImplementedException();
        }

        private void LDAINDY()
        {
            throw new NotImplementedException();
        }

        private void BCSREL()
        {
            throw new NotImplementedException();
        }

        private void LDXABS()
        {
            throw new NotImplementedException();
        }

        private void LDAABS()
        {
            throw new NotImplementedException();
        }

        private void LDYABS()
        {
            throw new NotImplementedException();
        }

        private void TAXIMP()
        {
            throw new NotImplementedException();
        }

        private void LDAIMM()
        {
            throw new NotImplementedException();
        }

        private void TAYIMP()
        {
            throw new NotImplementedException();
        }

        private void LDXZPG()
        {
            throw new NotImplementedException();
        }

        private void LDAZPG()
        {
            throw new NotImplementedException();
        }

        private void LDYZPG()
        {
            throw new NotImplementedException();
        }

        private void LDXIMM()
        {
            var contents = memory.Read(PC);
            X = contents;

            PC++;
        }

        private void LDAXIND()
        {
            throw new NotImplementedException();
        }

        private void LDYIMM()
        {
            throw new NotImplementedException();
        }

        private void STAABSX()
        {
            throw new NotImplementedException();
        }

        private void TXSIMP()
        {
            throw new NotImplementedException();
        }

        private void STAABSY()
        {
            throw new NotImplementedException();
        }

        private void TYAIMP()
        {
            throw new NotImplementedException();
        }

        private void STXZPGY()
        {
            throw new NotImplementedException();
        }

        private void STAZPGX()
        {
            throw new NotImplementedException();
        }

        private void STYZPGX()
        {
            throw new NotImplementedException();
        }

        private void STAINDY()
        {
            throw new NotImplementedException();
        }

        private void BCCREL()
        {
            throw new NotImplementedException();
        }

        private void STXABS()
        {
            var address = memory.ReadDouble(PC);

            memory.Write(address, X);

            PC++;
            PC++;
        }

        private void STAABS()
        {
            throw new NotImplementedException();
        }

        private void STYABS()
        {
            throw new NotImplementedException();
        }

        private void TXAIMP()
        {
            throw new NotImplementedException();
        }

        private void DEYIMP()
        {
            throw new NotImplementedException();
        }

        private void STXZPG()
        {
            throw new NotImplementedException();
        }

        private void STAZPG()
        {
            throw new NotImplementedException();
        }

        private void STYZPG()
        {
            throw new NotImplementedException();
        }

        private void STAXIND()
        {
            throw new NotImplementedException();
        }

        private void RORABSX()
        {
            throw new NotImplementedException();
        }

        private void ADCABSX()
        {
            throw new NotImplementedException();
        }

        private void ADCABSY()
        {
            throw new NotImplementedException();
        }

        private void SEIIMP()
        {
            SR |= Status.I;
        }

        private void RORZPGX()
        {
            throw new NotImplementedException();
        }

        private void ADCZPGX()
        {
            throw new NotImplementedException();
        }

        private void ADCINDY()
        {
            throw new NotImplementedException();
        }

        private void BVSREL()
        {
            throw new NotImplementedException();
        }

        private void RORABS()
        {
            throw new NotImplementedException();
        }

        private void ADCABS()
        {
            throw new NotImplementedException();
        }

        private void JMPIND()
        {
            throw new NotImplementedException();
        }

        private void RORA()
        {
            throw new NotImplementedException();
        }

        private void ADCIMM()
        {
            throw new NotImplementedException();
        }

        private void PLAIMP()
        {
            throw new NotImplementedException();
        }

        private void RORZPG()
        {
            throw new NotImplementedException();
        }

        private void ADCZPG()
        {
            throw new NotImplementedException();
        }

        private void ADCXIND()
        {
            throw new NotImplementedException();
        }

        private void RTSIMP()
        {
            throw new NotImplementedException();
        }

        private void LSRABSX()
        {
            throw new NotImplementedException();
        }

        private void EORABSX()
        {
            throw new NotImplementedException();
        }

        private void EORABSY()
        {
            throw new NotImplementedException();
        }

        private void CLIIMP()
        {
            throw new NotImplementedException();
        }

        private void LSRZPGX()
        {
            throw new NotImplementedException();
        }

        private void EORZPGX()
        {
            throw new NotImplementedException();
        }

        private void EORINDY()
        {
            throw new NotImplementedException();
        }

        private void BVCREL()
        {
            throw new NotImplementedException();
        }

        private void LSRABS()
        {
            throw new NotImplementedException();
        }

        private void EORABS()
        {
            throw new NotImplementedException();
        }

        private void JMPABS()
        {
            throw new NotImplementedException();
        }

        private void LSRA()
        {
            throw new NotImplementedException();
        }

        private void EORIM()
        {
            throw new NotImplementedException();
        }

        private void PHAIMP()
        {
            throw new NotImplementedException();
        }

        private void LSRZPG()
        {
            throw new NotImplementedException();
        }

        private void EORZPG()
        {
            throw new NotImplementedException();
        }

        private void EORXIND()
        {
            throw new NotImplementedException();
        }

        private void RTIIMP()
        {
            throw new NotImplementedException();
        }

        private void ROLABSX()
        {
            throw new NotImplementedException();
        }

        private void ANDABSX()
        {
            throw new NotImplementedException();
        }

        private void ANDABSY()
        {
            throw new NotImplementedException();
        }

        private void SECIMP()
        {
            throw new NotImplementedException();
        }

        private void ROLZPGX()
        {
            throw new NotImplementedException();
        }

        private void ANDINDY()
        {
            throw new NotImplementedException();
        }

        private void BMIREL()
        {
            throw new NotImplementedException();
        }

        private void ROLABS()
        {
            throw new NotImplementedException();
        }

        private void ANDABS()
        {
            throw new NotImplementedException();
        }

        private void BITABS()
        {
            
        }

        private void ROLZPG()
        {
            throw new NotImplementedException();
        }

        private void ROLA()
        {
            throw new NotImplementedException();
        }

        private void ANDIMM()
        {
            throw new NotImplementedException();
        }

        private void PLPIMP()
        {
            throw new NotImplementedException();
        }

        private void ANDZPG()
        {
            throw new NotImplementedException();
        }

        private void BITZPG()
        {
            throw new NotImplementedException();
        }

        private void ANDXIND()
        {
            throw new NotImplementedException();
        }

        private void JSRABS()
        {
            throw new NotImplementedException();
        }

        private void ASLABSX()
        {
            throw new NotImplementedException();
        }

        private void ORAABSX()
        {
            throw new NotImplementedException();
        }

        private void ORAABSY()
        {
            throw new NotImplementedException();
        }

        private void CLCIMP()
        {
            throw new NotImplementedException();
        }

        private void ASLZPGX()
        {
            throw new NotImplementedException();
        }

        private void ORAZPGX()
        {
            throw new NotImplementedException();
        }

        private void ORAINDY()
        {
            throw new NotImplementedException();
        }

        private void BPLREL()
        {
            throw new NotImplementedException();
        }

        private void ASLABS()
        {
            throw new NotImplementedException();
        }

        private void ORAABS()
        {
            throw new NotImplementedException();
        }

        private void ASLA()
        {
            throw new NotImplementedException();
        }

        private void ORAIMM()
        {
            throw new NotImplementedException();
        }

        private void PHPIMP()
        {
            throw new NotImplementedException();
        }

        private void CPXABS()
        {
            throw new NotImplementedException();
        }

        private void SBCABS()
        {
            throw new NotImplementedException();
        }

        private void INCABS()
        {
            throw new NotImplementedException();
        }

        private void BEQREL()
        {
            throw new NotImplementedException();
        }

        private void SBCXIND()
        {
            throw new NotImplementedException();
        }

        private void INCZPGX()
        {
            throw new NotImplementedException();
        }

        private void SBCZPGX()
        {
            throw new NotImplementedException();
        }

        private void SEDIMP()
        {
            throw new NotImplementedException();
        }

        private void SBCABSY()
        {
            throw new NotImplementedException();
        }

        private void SBCABSX()
        {
            throw new NotImplementedException();
        }

        private void INCABSX()
        {
            throw new NotImplementedException();
        }

        private void ASLZPG()
        {
            throw new NotImplementedException();
        }

        private void ORAZPG()
        {
            throw new NotImplementedException();
        }

        private void ORAXIND()
        {
            throw new NotImplementedException();
        }


        //private void RunResetHandler()
        //{
        //    var resetHandler = memory.ReadDouble();

        //    PC = resetHandler;


        //}

        private int cyclesToGo = 0;

        public void Tick()
        {
            if (cyclesToGo > 0)
            {
                cyclesToGo--;
                return;
            }

            var opcode = Fetch();
            var instructions = Instructions[opcode];

            cyclesToGo = instructions.Cycles - 1;
            instructions.Action();
        }

        private byte Fetch()
        {
            var fetched = memory.Read(PC);

            PC++;

            return fetched;
        }

        private ushort FetchDouble()
        {
            var low = Fetch();
            var high = (ushort)(Fetch() << 8);

            return (ushort)(high | low);
        }

        private void Push(byte value)
        {
            
        }

        private void Push(ushort value)
        {
            var hi = (byte)(value >> 8);
            var lo = (byte)(value & 0xFF);

            Push(hi);
            Push(lo);
        }

        private void PushStatus()
        {
            Push((byte)SR);
        }

        private void BRKIMP()
        {
            Push(PC);
            PushStatus();
        }

        private static void FLT()
        {
            throw new InvalidOperationException("Instruction does not exist for opcode");
        }

       
    }
}
