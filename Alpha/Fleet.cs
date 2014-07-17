using System;
using Alpha.Scripting;
using Alpha.WorldGeneration;

namespace Alpha
{
    interface IFleet : IScriptableFleet
    { }
    class Fleet : IFleet
    {
        public Realm Owner { get; set; }
        public int ShipCount { get; set; }
        public String Name { get; set; }
        public VoronoiSite Location { get; set; }
        public Double Speed { get; set; } //Expressed as distance-unit per day

        public override string ToString()
        {
            return Name + " (" + ShipCount + " ships)";
        }
    }
}
