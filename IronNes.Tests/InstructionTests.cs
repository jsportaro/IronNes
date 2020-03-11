using System;
using Xunit;

namespace IronNes.Tests
{
    public class InstructionTests
    {
        [Fact]
        public void SEI()
        {
            var (cpu, memory, clock) = CreateMachine();

            clock.TickTill(1000, (cpu) =>
            {
                return cpu.PC == 0x801D;
            });

            Assert.True((cpu.SR & Status.I) == Status.I);
        }

        [Fact]
        public void CLD()
        {
            var (cpu, memory, clock) = CreateMachine();

            clock.TickTill(1000, (cpu) =>
            {
                return cpu.PC == 0x801E;
            });

            Assert.True((cpu.SR & Status.D) == 0);
        }

        [Fact]
        public void LDX_Immediate()
        {
            var (cpu, memory, clock) = CreateMachine();

            var cycles = clock.TickTill(1000, (cpu) =>
            {
                return cpu.PC == 0x8020;
            });

            Assert.True(cpu.X == 0);
        }

        [Fact]
        public void STX_Absolute()
        {
            var (cpu, memory, clock) = CreateMachine();

            var cycles = clock.TickTill(1000, (cpu) =>
            {
                return cpu.PC == 0x8023;
            });

            Assert.True(memory.Read(0x2000) == 0);
        }


        private (Cpu, Memory, TestClock) CreateMachine()
        {
            var memory = new Memory(GamePakLibrary.Read("helloworld.nes"));
            var cpu = new Cpu(memory);
            var clock = new TestClock(cpu);

            return (cpu, memory, clock);
        }
    }
}
