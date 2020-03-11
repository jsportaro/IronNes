using System;
using System.Collections.Generic;
using System.Text;

namespace IronNes.Tests
{
    public class TestClock
    {
        private readonly Cpu cpu;
        public TestClock(Cpu cpu)
        {
            this.cpu = cpu;
        }

        public long TickTill(int timeout, Func<Cpu, bool> tickPredicate)
        {
            long ticks = 0;
            for ( ; timeout >= 0; ticks++, timeout--)
            {
                cpu.Tick();

                if (tickPredicate(cpu))
                {
                    return ticks;
                }
            }

            throw new TimeoutException();
        }
    }
}
