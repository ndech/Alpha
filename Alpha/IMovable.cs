using System;
using System.Collections.Generic;

namespace Alpha
{
    interface IMovable
    {
        Province Location { get; set; }
        Func<Province, bool> CanCross { get; }
        float Speed { get; }
        void SetMoveOrder(IGame game, List<MoveOrder.Step> calculatePath);
    }
}