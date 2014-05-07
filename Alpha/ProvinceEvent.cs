using System;
using System.Collections.Generic;

namespace Alpha
{
    class ProvinceEvent : Event<Province>
    {
        private List<Func<Province, float>> Multipliers;

        public ProvinceEvent()
        {
            Multipliers = new List<Func<Province, float>>();
            Multipliers.Add(p => p.Population > 1150 ? 0.1f : 1);
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
