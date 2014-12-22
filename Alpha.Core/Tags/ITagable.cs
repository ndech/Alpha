using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Core.Tags
{
    interface ITagable
    {
        TagCollection Tags { get; }
    }
}
