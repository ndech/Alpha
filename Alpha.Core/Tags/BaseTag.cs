using System;
using System.Xml.Linq;

namespace Alpha.Core.Tags
{
    class BaseTag : Tag
    {
        public string Key { get; }

        internal BaseTag(string key)
        {
            Key = key;
        }

        public override bool Equals(Tag other)
        {
            return other is BaseTag && ((BaseTag) other).Key.Equals(Key);
        }

        public override XElement Save() => new XElement("baseTag", Key);

        public override void Load()
        {
            throw new NotImplementedException();
        }

        internal override bool IsValid() => true;

        public override string ToString() => "BaseTag : " + Key;
    }
}
