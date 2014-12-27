using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Core.Tags
{
    public abstract class Tag : IEquatable<Tag>
    {
        public abstract bool Equals(Tag other);
    }
}
