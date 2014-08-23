using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alpha.Core;
using Alpha.Core.Commands;
using Alpha.Core.Fleets;
using Alpha.Core.Notifications;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.AI
{
    public class Ai : IAi
    {
        private readonly Realm _realm;
        private readonly IWorld _world;

        public Ai(Realm realm, IWorld world)
        {
            _realm = realm;
            _world = world;
        }

        public IList<Command> Process(DataLock dataLock, IEnumerable<Notification> notifications)
        {
            List<Command> commands = new List<Command>();
            Thread.Sleep(RandomGenerator.Get(100, 1000));

            Console.WriteLine("IA calculations done for " + _realm);
            IEnumerable<Fleet> myFleets = _world.FleetManager.Fleets.Where(f => f.Owner == _realm);
            foreach (Fleet fleet in myFleets)
            {
                if(!fleet.HasMoveOrder)
                    commands.Add(new MoveFleetCommand(fleet, _world.ProvinceManager.CalculatePath(fleet, _world.ProvinceManager.SeaProvinces.RandomItem())));
            }
            return commands;
        }

    }
}
