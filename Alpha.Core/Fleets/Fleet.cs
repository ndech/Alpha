using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alpha.Core.Movement;
using Alpha.Core.Notifications;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;

namespace Alpha.Core.Fleets
{
    public class Fleet : Component, IDailyUpdatableItem, IMovable
    {
        internal Fleet(World world, String name, Realm owner, Province location, List<Ship> ships) : base(world)
        {
            Id = IdSequence;
            Name = name;
            Owner = owner;
            Location = location;
            _ships = ships;
        }
        public String Name { get; internal set; }
        private readonly List<Ship> _ships = new List<Ship>();
        public IEnumerable<Ship> Ships { get { return _ships; } }
        public int ShipCount { get { return _ships.Count; } }
        public Realm Owner { get; internal set; }
        public Province Location { get; private set; }
        public Func<Province, bool> CanCross { get { return province => province is SeaProvince; } }
        public float Speed { get { return _ships.Min(s => s.Speed); } }
        void IMovable.SetLocation(Province location)
        {
            Location = location;
            World.Notify(new MoveFleetNotification(this));
        }

        private IMoveOrder _moveOrder;

        public IMoveOrder MoveOrder
        {
            get
            {
                return _moveOrder;
            }
            internal set
            {
                World.Notify(new MoveFleetNotification(this));
                _moveOrder = value;
            }
        }

        public bool HasMoveOrder { get { return MoveOrder != null; } }

        private static int _idSequence;
        protected static int IdSequence { get { return ++_idSequence; } }
        public int Id { get; private set; }
        void IDailyUpdatableItem.DayUpdate()
        {
            Console.WriteLine("Update fleet "+Name);
            Thread.Sleep(10);
            if(HasMoveOrder)
                ((IDailyUpdatableItem)MoveOrder).DayUpdate();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
