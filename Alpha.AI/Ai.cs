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
        private readonly RealmToken _realm;
        private readonly IWorld _world;

        public Ai(RealmToken realm, IWorld world)
        {
            _realm = realm;
            _world = world;
        }

        public IEnumerable<Command> Process(DataLock dataLock, IEnumerable<Notification> notifications)
        {
            List<Command> commands = new List<Command>();
            //for (int i = 0; i < 10; i++)
            //{
            //    dataLock.AiRead(() => Thread.Sleep(RandomGenerator.Get(1, 10)));
            //}
            commands.AddRange(dataLock.AiRead<List<Command>>(ProcessFleets));
            DebugConsole.WriteLine("IA calculations done for " + _realm);
            return commands;
        }

        private List<Command> ProcessFleets()
        {
            List<Command> commands = new List<Command>();
            IEnumerable<Fleet> myFleets = _world.FleetManager.Fleets.Where(f => f.Owner == _realm);
            foreach (Fleet fleet in myFleets)
            {
                if (!fleet.HasMoveOrder || RandomGenerator.Get(0, 10) == 0)
                {
                    var steps = _world.ProvinceManager.CalculatePath(fleet,
                        _world.ProvinceManager.SeaProvinces.Where(p => p != fleet.Location.Province).RandomItem().Zones.RandomItem());
                    if(steps.Count>0) // If steps.Count == 0 => path not found
                        commands.Add(new MoveFleetCommand(fleet,steps));
                }
            }
            return commands;
        }

        public Realm Realm { get { return _realm; } }
        public RealmToken RealmToken { get { return _realm; } }
    }
}
