using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha
{
    abstract class Territory
    {
        public string Name { get; set; }

        protected float _population;
        public Int32 Population
        {
            get { return (int)_population; }
            set
            { _population = Math.Max(0.0f, _population + value - Population); }
        }
        public float BaseTax { get { return Population * 0.1f; } }
    }
}
