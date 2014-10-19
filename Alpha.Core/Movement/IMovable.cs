using System;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    interface IMovable
    {
        Zone Location { get; }
        Func<Zone, bool> CanCross { get; }
        float Speed { get; }
        void SetLocation(Zone location);
        void EndMovement();
    }
}