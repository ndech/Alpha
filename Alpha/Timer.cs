using Alpha.Toolkit.IO;

namespace Alpha
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    public class Timer : ICsvLoggable
    {
        private readonly Stopwatch _stopWatch;
        private Int64 _elapsedMilliseconds;
        private Int64 _deltaMilliseconds;

        public Timer()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        public double Tick()
        {
            Int64 currentElapsed = _stopWatch.ElapsedMilliseconds;
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

        public override String ToString()
        {
            return _elapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
        }

        public string ToCsv()
        {
            return _elapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}