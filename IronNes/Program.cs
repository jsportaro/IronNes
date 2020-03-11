using System;

namespace IronNes
{
    class Program
    {
        static void Main(string[] args)
        {
            var memory = new Memory(null);

            memory.Write(0, 0xA9);
            memory.Write(1, 0x01);
            memory.Write(2, 0x8D);
            memory.Write(3, 0x00);
            memory.Write(4, 0x02);

            var executor = new Cpu(memory);
            var clock = new Clock(new IClockable[] { executor }, 1790000);
            //var clock = new Clock(new IClockable[] { executor }, 100);
            var debugger = new Debugger(clock);

            clock.Start();
            System.Threading.Thread.Sleep(1);
            clock.Stop();

            clock.Start();
            System.Threading.Thread.Sleep(5000);
            clock.Stop();

            debugger.Run(executor);
        }
    }
}
