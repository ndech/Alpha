using System;
using System.Collections.Generic;
using Alpha.Graphics;
using Alpha.Scripting;

namespace Alpha
{
    interface IFleet : IScriptableFleet
    { }
    class Fleet : IFleet, IMovable, IUpdatable, IDailyUpdatable, ISelectable
    {
        public Fleet(Realm owner, int shipCount, string name, Province location, float speed)
        {
            Owner = owner;
            ShipCount = shipCount;
            Name = name;
            Location = location;
            Speed = speed;
        }

        public Realm Owner { get; set; }
        public int ShipCount { get; set; }
        public String Name { get; set; }
        public Province Location { get; set; }
        public float Speed { get; set; }
        public bool Selected { get; set; }

        public Func<Province, bool> CanCross
        {
            get { return province => province is SeaProvince; }
        }

        public void SetMoveOrder(IGame game, List<MoveOrder.Step> steps)
        {
            MoveOrder = steps == null ? null : new FleetMoveOrder(game.Services.Get<IRenderer>(), this, steps);
        }

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
        IScriptableProvince IScriptableFleet.Location { get { return Location; } }
        public void Select()
        {
            Selected = true;
        }

        public void UnSelect()
        {
            Selected = false;
        }
    }
}
