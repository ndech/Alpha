using System.Xml.Linq;

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

        internal override bool IsValid() => _duration > 0;

        public override XElement Save()
        {
            return new XElement("timerTag", 
                new XElement("duration", _duration),
                new XElement("key", Key));
        }
    }
}
