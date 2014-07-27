using System;
using System.Collections.Generic;
using Alpha.Scripting;

namespace Alpha
{
    interface IFleet : IScriptableFleet
    { }
    class Fleet : IFleet, IMovable, IUpdatable, IDailyUpdatable, ISelectable
    {
        public Realm Owner { get; set; }
        public int ShipCount { get; set; }
        public String Name { get; set; }
        public Province Location { get; set; }

        public Func<Province, bool> CanCross
        {
            get { return province => province is SeaProvince; }
        }

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
