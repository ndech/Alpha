using System;
using System.Collections.Generic;

namespace Alpha.Core.Fleets
{
    public interface IFleet
    {
        IEnumerable<IShip> Ships { get; }
        String Name { get; }
    }

    partial class Fleet
    {
        IEnumerable<IShip> IFleet.Ships { get { return Ships; } } 
    }
}
