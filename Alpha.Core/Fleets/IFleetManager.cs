using System.Collections.Generic;

namespace Alpha.Core.Fleets
{
    public interface IFleetManager
    {
        IEnumerable<IFleet> Fleets { get; }
    }
}