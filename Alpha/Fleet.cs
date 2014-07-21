using System;
using Alpha.Scripting;
using Alpha.WorldGeneration;

namespace Alpha
{
    interface IFleet : IScriptableFleet
    { }
    class Fleet : IFleet, IUpdatable, IDailyUpdatable
    {
        public Realm Owner { get; set; }
        public int ShipCount { get; set; }
        public String Name { get; set; }
        public VoronoiSite Location { get; set; }
        public float Speed { get; set; }
        public float Angle { get; set; }
        public FleetMoveOrder MoveOrder { get; set; }
        public override string ToString()
        {
            return Name + " (" + ShipCount + " ships)";
        }

        public void DayUpdate()
        {
            if (MoveOrder != null)
                MoveOrder.DayUpdate();
        }

        public void Update(double delta)
        {
            if(MoveOrder!=null)
                MoveOrder.Update(delta);
        }
    }
}
