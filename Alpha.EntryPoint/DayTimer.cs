﻿using System.Diagnostics;
using System.Threading;

namespace Alpha.EntryPoint
{
    class DayTimer
    {
        private const int SecondsPerDay = 5;
        private readonly Stopwatch _stopWatch;
        internal DayTimer()
        {
            _stopWatch = new Stopwatch();
        }

        public void Start()
        {
            _stopWatch.Restart();
        }

        public void WaitForNextDay()
        {
            while(_stopWatch.ElapsedMilliseconds < SecondsPerDay*1000)
                Thread.Sleep(25);
            _stopWatch.Restart();
        }
    }
}
