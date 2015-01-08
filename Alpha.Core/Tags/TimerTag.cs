namespace Alpha.Core.Tags
{
    class TimerTag : BaseTag, IDailyUpdatableItem
    {
        private int _duration;

        internal TimerTag(string key, int duration) : base(key)
        {
            _duration = duration;
        }

        public void DayUpdate()
        {
            _duration--;
        }

        public bool IsValid()
        {
            return _duration > 0;
        }
    }
}
