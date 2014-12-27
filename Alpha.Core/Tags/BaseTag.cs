using System;

namespace Alpha.Core.Tags
{
    class BaseTag : Tag
    {
        public String Key { get; private set; }

        internal BaseTag(String key)
        {
            Key = key;
        }

        public override bool Equals(Tag other)
        {
            return other is BaseTag && ((BaseTag) other).Key.Equals(Key);
        }

        public override string ToString()
        {
            return "BaseTag : " + Key;
        }
    }
}
