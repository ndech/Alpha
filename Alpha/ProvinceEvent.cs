using System;
using System.Collections.Generic;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using Alpha.Toolkit;

namespace Alpha
{
    class ProvinceEvent
    {
        private readonly List<Func<IProvince, float>> _multipliers;
        private readonly Action<IProvince> _effect = delegate(IProvince province) {  };

        public ProvinceEvent()
        {
            _multipliers = new List<Func<IProvince, float>>();

            ScriptEngine engine = new ScriptEngine();
            Session session = Session.Create();
            session.AddReference(typeof(IProvince).Assembly);
            //_multipliers.Add(engine.Execute<Func<IProvince, float>>("(p) => p.Population > 1150 ? 0.1f : 1", session));
            //_effect = engine.Execute<Action<IProvince>>("(p) => " + "p.Population += 100", session);
        }

        public void Process(IProvince province)
        {
            float meanTimeToHappen = 10;
            foreach (Func<IProvince, float> multiplier in _multipliers)
                meanTimeToHappen *= multiplier(province);
            if (RandomGenerator.Get(0, (int)meanTimeToHappen) == 0)
                _effect(province);
        }
    }
}
