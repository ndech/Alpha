namespace Alpha.Toolkit
{
    public class FpsCounter : ICounter
    {
        public float Value { get; private set; }
        public string Unit {
            get { return "FPS"; }
        }
        private double _timeCounter;
        private int _frameCount;

        public FpsCounter()
        {
            _timeCounter = 0;
            _frameCount = 0;
            Value = 0;
        }
        public void Update(double seconds)
        {
            _frameCount++;
            _timeCounter += seconds;
            if (_timeCounter > 1.0)
            {
                Value = _frameCount;
                _frameCount = 0;
                _timeCounter = 0;
            }
        }
    }
}
