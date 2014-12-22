using System.Collections.Generic;

namespace Alpha.Core.Tags
{
    public class TagCollection
    {
        private List<Tag> Tags; 

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
            Tags.RemoveAll(t=> t.Equals(tag));
            return true;
        }
    }
}
