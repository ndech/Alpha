using System.Collections;
using System.Collections.Generic;

namespace Alpha.Core.Tags
{
    public class TagCollection : IEnumerable<Tag>
    {
        private HashSet<Tag> Tags;

        public void Add(Tag tag)
        {
            Tags.Add(tag);
        }

        public bool Contains(Tag tag)
        {
            return Tags.Contains(tag);
        }

        public bool Pop(Tag tag)
        {
            if (!Contains(tag))
                return false;
            Tags.Remove(tag);
            return true;
        }

        public IEnumerator<Tag> GetEnumerator()
        {
            return Tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
