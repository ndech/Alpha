using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Scripting
{
    public interface IScriptableRealm : IEventable
    {
        String Name { get; }
        Int32 DirectVassalCount { get; }
        Int32 TotalVassalCount { get; }
        bool IsIndependant { get; }
        double TaxRate { get; }
    }
}