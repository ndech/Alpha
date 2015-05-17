using System;
using System.Diagnostics;
using System.Globalization;

namespace Alpha.Toolkit
{
    public class Timer
    {
        private readonly Stopwatch _stopWatch;
        private long _elapsedMilliseconds;
        private long _deltaMilliseconds;

        public Timer()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public double Tick()
        {
            long currentElapsed = _stopWatch.ElapsedMilliseconds;
            _deltaMilliseconds = currentElapsed - _elapsedMilliseconds;
            _elapsedMilliseconds = currentElapsed;
            return Delta;
        }

        public double Delta
        {
            get { return  _deltaMilliseconds/1000.0; }
        }

        public double Elapsed
        {
            get { return _elapsedMilliseconds/1000.0; }
        }

        public override string ToString()
        {
            return _elapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}