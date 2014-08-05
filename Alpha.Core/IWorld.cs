using Alpha.Core.Fleets;

namespace Alpha.Core
{
    interface IWorld
    {
        IFleetManager Fleets { get; }
    }
}