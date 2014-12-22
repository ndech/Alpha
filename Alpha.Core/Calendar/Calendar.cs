using System.Collections.Generic;
using Alpha.Core.Tags;
using Alpha.Toolkit;

namespace Alpha.Core.Calendar
{
    public class Calendar : Manager, ITagable
    {
        public Season CurrentSeason { get; private set; }
        public int CurrentSeasonLength { get; private set; }
        public Date CurrentDate { get; private set; }
        public TagCollection Tags { get; set; }

        internal Calendar(World world) : base(world)
        {
            CurrentDate = new Date(1,1,1900);
            Tags = new TagCollection();
            _seasons = new List<Season>();
        }

        internal override void DayUpdate(DataLock dataLock)
        {
            dataLock.Write(() =>
            {
                CurrentDate = CurrentDate.NextDay();
                CurrentSeasonLength++;
            });
        }

        internal override void Initialize()
        { }

        internal int AgeOf(Date birthdate)
        {
            return CurrentDate.Year - birthdate.Year;
        }

        private IEnumerable<Season> _seasons; 
    }
}
