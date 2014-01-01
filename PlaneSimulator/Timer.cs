using PlaneSimulator.Toolkit.IO;

namespace PlaneSimulator
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    public class Timer : ICsvLoggable
    {
        private Stopwatch stopWatch;
        private Int64 elapsedMilliseconds;
        private Int64 deltaMilliseconds;

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
            get { return ((double) deltaMilliseconds/1000.0); }
        }

        public double Elapsed
        {
            get { return (double) elapsedMilliseconds/1000.0; }
        }

        public override String ToString()
        {
            return elapsedMilliseconds.ToString();
        }

        public string ToCsv()
        {
            return elapsedMilliseconds.ToString();
        }
    }
}