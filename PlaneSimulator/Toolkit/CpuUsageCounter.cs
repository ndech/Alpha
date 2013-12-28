using System;
using System.Diagnostics;

namespace PlaneSimulator.Toolkit
{
    class CpuUsageCounter : IDisposable, IUpdatable
    {
        public float Value 
		{
			get
			{
				return _canReadCpu ? _lastCpuUsage : 0;
			}
		}

        private readonly bool _canReadCpu;
        private readonly PerformanceCounter _counter;
        private double timeCounter;
		float _lastCpuUsage;

        public CpuUsageCounter()
		{
			_canReadCpu = true;
		    _lastCpuUsage = 0;
            timeCounter = 0;

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
		    timeCounter += seconds;
		    if (timeCounter > 1)
            {
                _lastCpuUsage = _counter.NextValue();
                timeCounter %= 1;
            }
		}
	}
}
