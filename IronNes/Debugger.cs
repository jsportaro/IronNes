using System;
using System.Collections.Generic;
using System.Text;

namespace IronNes
{
    public class Debugger
    {
        private List<ushort> breakpoints = new List<ushort>();
        private bool inStep = false;
        private Clock clock;
        public Debugger(Clock clock)
        {
            this.clock = clock;
        }

        public void Set(ushort breakpoint)
        {
            breakpoints.Add(breakpoint);
        }

        public bool Suspend(ushort pc)
        {
            return breakpoints.Contains(pc) || inStep;
        }

        internal void Run(Cpu executor)
        {
            clock.Start();


        }
    }
}
