using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IronNes
{
    public delegate void TickEventHandler(object sender, EventArgs args);

    public class Clock
    {
        private Stopwatch master = new Stopwatch();
        private IEnumerable<IClockable> clockables;
        private double hertz;
        private double millisecondPerCycle;

        public event TickEventHandler TickEvent;

        public long EmulatedTicks { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clockables"></param>
        /// <param name="frequency">In hertz</param>
        public Clock(IEnumerable<IClockable> clockables, double hertz)
        {
            this.clockables = clockables;
            this.hertz = hertz;
            millisecondPerCycle = 1000 / hertz;
        }

        private bool halt = false;
        private Task loopTask;
        public void Start()
        {
            if (loopTask != null)
            {
                loopTask.Dispose();
                loopTask = null;
            }
            halt = false;
            loopTask = Task.Run(() => Loop());
            
        }

        public TimeSpan Stop()
        {
            halt = true;

            //  If you don't let the loop finish
            //  before resetting the clock, you
            //  can enter an "infinite" loop
            Task.WaitAll(loopTask);

            return elapsedRuntime;
        }

        private TimeSpan elapsedRuntime;

        private void Loop()
        {
            var sw = new Stopwatch();
            EmulatedTicks = 0;
            if (!Stopwatch.IsHighResolution)
                throw new NotSupportedException("Need a high resolution stopwatch");

            if (hertz > Stopwatch.Frequency)
                throw new NotSupportedException("Stopwatch can not handle hertz of that magnitude");

            var ticksPerCycle = Stopwatch.Frequency / hertz;
            master.Start();
            for (; !halt;)
            {
                
                var blockTillTickCount = master.ElapsedTicks + ticksPerCycle;
                //sw.Start();

                //foreach (var clockable in clockables)
                //{
                //    clockable.Tick();
                //}

                //sw.Stop();


                //SpinWait.SpinUntil(() => master.ElapsedTicks >= blockTillTickCount); // This will destroy the CPU
                while (master.ElapsedTicks < blockTillTickCount) { }
                
                //

                //sw.Reset();
                EmulatedTicks++;
            }

            master.Stop();

            elapsedRuntime = master.Elapsed;

            master.Reset();
        }
    }
}
