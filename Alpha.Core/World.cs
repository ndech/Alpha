using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Alpha.Common;
using Alpha.Core.Calendars;
using Alpha.Core.Commands;
using Alpha.Core.Fleets;
using Alpha.Core.Notifications;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.Core
{
    public class World : IProcessableWorld, IWorld
    {
        private readonly List<Notification> _dailyNotifications;
        private readonly List<Notification> _liveNotifications;
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private List<IManager> Managers { get; set; }
        public FleetManager FleetManager { get; private set; }
        public RealmManager RealmManager { get; private set; }
        public ProvinceManager ProvinceManager { get; private set; }
        public Calendar Calendar { get; private set; }

        internal World(List<Notification> dailyNotifications, List<Notification> liveNotifications)
        {
            _dailyNotifications = dailyNotifications;
            _liveNotifications = liveNotifications;
            RealmManager = new RealmManager();
            FleetManager = new FleetManager();
            ProvinceManager = new ProvinceManager();
            Calendar = new Calendar();
            Managers = new List<IManager> { Calendar, FleetManager, RealmManager, ProvinceManager };
        }

        public void RegisterCommand(Command command)
        {
            Console.WriteLine(command);
            _commands.Enqueue(command);
        }

        public void RegisterCommands(IEnumerable<Command> commands)
        {
            foreach (Command command in commands)
                RegisterCommand(command);
        }

        private void DayUpdate(DataLock datalock)
        {
            Managers.ForEach(u => u.DayUpdate(datalock));
        }

        private void ProcessCommands(DataLock datalock)
        {
            while (true)
            {
                Command command;
                if (!_commands.TryDequeue(out command)) return;
                datalock.Write(() =>
                {
                    if (command.IsValid())
                        command.Execute();
                });
            }
        }

        void IProcessableWorld.Process(DataLock dataLock)
        {
            ProcessCommands(dataLock);
            DayUpdate(dataLock);
        }
    }
}