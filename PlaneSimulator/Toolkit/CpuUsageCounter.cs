using System;
using System.Diagnostics;

namespace PlaneSimulator.Toolkit
{
    class CpuUsageCounter : IDisposable, ICounter
    {
        public float Value 
		{
			get
			{
				return _canReadCpu ? _lastCpuUsage : 0;
			}
		}

        public string Unit { get { return "%"; } }

        private readonly bool _canReadCpu;
        private readonly PerformanceCounter _counter;
        private double _timeCounter;
		private float _lastCpuUsage;

        public CpuUsageCounter()
		{
			_canReadCpu = true;
		    _lastCpuUsage = 0;
            _timeCounter = 0;

			try
			{
				_counter = new PerformanceCounter
				{
				    CategoryName = "Processor",
				    CounterName = "% Processor Time",
				    InstanceName = "_Total"
				};
			}
			catch
			{
				_canReadCpu = false;
			}
		}

		public void Dispose()
		{
			if(_canReadCpu)
				_counter.Close();
		}

		public void Update(double seconds)
		{
		    if (!_canReadCpu) return;
		    _timeCounter += seconds;
		    if (_timeCounter > 2)
            {
                _lastCpuUsage = _counter.NextValue();
                _timeCounter %= 1;
            }
		}
	}
}
