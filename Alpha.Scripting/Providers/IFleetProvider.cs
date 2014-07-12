using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Scripting.Providers
{
    public interface IFleetProvider
    {
        IList<IScriptableFleet> ScripableFleets { get; } 
    }
}
