using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    class Timer
    {
        Stopwatch stopWatch;
        Int64 elapsedMilliseconds;
        Int64 deltaMilliseconds;

        public void Initialize()
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        public void Tick()
        {
            Int64 currentElapsed = stopWatch.ElapsedMilliseconds;
            deltaMilliseconds = currentElapsed - elapsedMilliseconds;
            elapsedMilliseconds = currentElapsed;
        }

        public double Delta 
        {
            get
            {
                return ((double)deltaMilliseconds / 1000.0);
            }
        }
        
        public double Elapsed
        {
            get
            {
                return (double)elapsedMilliseconds / 1000.0;
            }
        }

        public override String ToString()
        {
            int framePerSeconds;
            if (deltaMilliseconds == 0)
                framePerSeconds = 0;
            else
                framePerSeconds = (int)(1000.0 / deltaMilliseconds);
            return string.Format(
                CultureInfo.CurrentCulture,
                "Time : {0} - Frames per seconds : {1}",
                elapsedMilliseconds, framePerSeconds);
        }
    }
}
