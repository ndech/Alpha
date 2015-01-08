using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Alpha.Toolkit;

namespace Alpha.Core.Tags
{
    public class TagCollection : IEnumerable<Tag>, IDailyUpdatable
    {
        private readonly HashSet<Tag> _tags;

        internal TagCollection()
        {
            _tags = new HashSet<Tag>();
        }

        public void Add(Tag tag)
        {
            _tags.Add(tag);
        }

        public bool Contains(Tag tag)
        {
            return _tags.Contains(tag);
        }

        public bool Pop(Tag tag)
        {
            if (!Contains(tag))
                return false;
            _tags.Remove(tag);
            return true;
        }

        public int PopWhere(Predicate<Tag> match)
        {
            return _tags.RemoveWhere(match);
        }

        public IEnumerator<Tag> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void DayUpdate(DataLock dataLock)
        {
            dataLock.Write(()=>
            {
                _tags.OfType<TimerTag>().DayUpdate();
                _tags.RemoveWhere(t => (t is TimerTag && !((TimerTag) t).IsValid()));
            });
        }
    }
}
