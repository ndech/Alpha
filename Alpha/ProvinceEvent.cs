using System;
using System.Collections.Generic;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
namespace Alpha
{
    class ProvinceEvent : Event<Province>
    {
        private List<Func<IProvince, float>> Multipliers;

        public ProvinceEvent()
        {
            Multipliers = new List<Func<IProvince, float>>();

            ScriptEngine engine = new ScriptEngine();
            Session session = Session.Create();
            session.AddReference(typeof(IProvince).Assembly);
            Multipliers.Add(engine.Execute<Func<IProvince, float>>("(p) => p.Population > 1150 ? 0.1f : 2", session));
        }

        public override void Process(Province item)
        {
            float meanTimeToHappen = 10;
            foreach (Func<Province, float> multiplier in Multipliers)
                meanTimeToHappen *= multiplier.Invoke(item);
            if (Random.Generator.Get(0, (int)meanTimeToHappen) == 0)
                item.YearlyGrowth += 0.1f;
        }
    }
}
