using System;
using System.Xml.Linq;
using Alpha.Core.Save;

namespace Alpha.Core.Tags
{
    public abstract class Tag : IEquatable<Tag>, ISavable
    {
        public abstract bool Equals(Tag other);
        public abstract XElement Save();
        public abstract void Load();
        internal abstract bool IsValid();
    }
}
