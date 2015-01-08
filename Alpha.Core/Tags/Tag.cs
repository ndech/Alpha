using System;

namespace Alpha.Core.Tags
{
    public abstract class Tag : IEquatable<Tag>
    {
        public abstract bool Equals(Tag other);
    }
}
