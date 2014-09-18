using System;
using System.Collections.Generic;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    interface IMovable
    {
        Province Location { get; }
        Func<Province, bool> CanCross { get; }
        float Speed { get; }
        void SetLocation(Province location);
        void EndMovement();
    }
}