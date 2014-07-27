using System;
using System.Collections.Generic;

namespace Alpha
{
    interface IMovable
    {
        Province Location { get; }
        Func<Province, bool> CanCross { get; }
        float Speed { get; }
    }
}
