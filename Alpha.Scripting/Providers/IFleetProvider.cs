using System.Collections.Generic;

namespace Alpha.Scripting.Providers
{
    public interface IFleetProvider
    {
        IList<IScriptableFleet> ScripableFleets { get; } 
    }
}
