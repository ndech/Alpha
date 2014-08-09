using System;
using System.Collections.Generic;

namespace Alpha.Scripting
{
    public interface IScriptableRealm : IEventable
    {
        String Name { get; }
        Int32 DirectVassalCount { get; }
        Int32 TotalVassalCount { get; }
        bool IsIndependant { get; }
        double TaxRate { get; }
        double Treasury { get; set; }
        double Income(String timeSpan);
        IScriptableRealm Liege { get; }
        IScriptableRealm RandomDirectVassal { get; }
        IScriptableRealm RandomDirectVassalWhere(Func<IScriptableRealm, bool> criteria);
        IList<IScriptableProvince> Demesne { get; }
        Int32 DemesneSize { get; } 

    }
}