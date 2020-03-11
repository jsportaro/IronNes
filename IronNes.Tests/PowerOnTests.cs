using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IronNes.Tests
{
    public class PowerOnTests
    {
        [Fact]
        public void ShouldStartAtResetHandler()
        {
            var memory = new Memory(GamePakLibrary.Read("helloworld.nes"));
            var cpu = new Cpu(memory);
            var clock = new TestClock(cpu);

            clock.TickTill(1000, (cpu) =>
            {
                return cpu.PC == 0x801D;
            });
        }

        [Fact]
        public void ShouldClearStatusRegisterBeforeFirstTick()
        {
            var memory = new Memory(GamePakLibrary.Read("helloworld.nes"));
            var cpu = new Cpu(memory);

            Assert.True(((byte)cpu.SR & 0xF) == 0);
        }
    }
}
