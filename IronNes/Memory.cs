using System;
using System.Collections.Generic;
using System.Text;

namespace IronNes
{
    public class Memory
    {
        private const ushort PrgRomAddressOne = 0x8000;

        private const ushort MAX_ADDRESS = UInt16.MaxValue - 1;
        private const ushort MIN_ADDRESS = 0;

        private byte[] gamePak;
        private byte[] ppuRegisters = new byte[8];

        public void Load(byte[] gamePak)
        {
            this.gamePak = gamePak;
        }

        private byte[] memory = new byte[MAX_ADDRESS];

        public Memory(byte[] gamePak)
        {
            this.gamePak = gamePak;
        }

        public byte Read(ushort address)
        {
            var (mappedAddress, bank) = MapAddress(address);

            return bank[mappedAddress];
        }

        private (int, byte []) MapAddress(ushort address)
        {
            if (address > 0x8000)  // GamePak memory
            {
                var mappedAddress = (address - 0x7ff0);

                return (mappedAddress, gamePak);
            } 
            else if (address >= 0x2000 && address <= 0x3FFF)  //PPU Registers
            {
                var registerIndex = address % 8;

                return (registerIndex, ppuRegisters);
            }

            return (address, memory);
        }

        public ushort ReadDouble(ushort address)
        {
            var (mappedAddress, bank) = MapAddress(address);

            var hi = bank[mappedAddress + 1];
            var lo = bank[mappedAddress];

            return (ushort)((hi << 8) | lo);
        }

        public void Write(ushort address, byte value)
        {
            var (mappedAddress, bank) = MapAddress(address);

            bank[mappedAddress] = value;
        }
    }
}
