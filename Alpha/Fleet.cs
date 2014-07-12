using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.Scripting;

namespace Alpha
{
    class Fleet : IScriptableFleet
    {
        public Realm Owner { get; set; }
        public int ShipCount { get; set; }
        public String Name { get; set; }

        public override string ToString()
        {
            return Name + " (" + ShipCount + " ships)";
        }
    }
}
