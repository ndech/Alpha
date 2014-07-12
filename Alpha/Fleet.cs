using System;
using Alpha.Scripting;

namespace Alpha
{
    interface IFleet : IScriptableFleet
    { }
    class Fleet : IFleet
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
