using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Alpha.Common;
using Alpha.Core.Commands;
using Alpha.Core.Events;
using Alpha.Core.Fleets;
using Alpha.Core.Notifications;
using Alpha.Core.Provinces;
using Alpha.Core.Realms;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Core
{
    public class World : IProcessableWorld, IWorld
    {
        private readonly List<Notification> _dailyNotifications;
        private readonly List<Notification> _liveNotifications;
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private HashSet<RealmToken> _interactiveModeRealms = new HashSet<RealmToken>();
        private List<Manager> Managers { get; set; }
        public FleetManager FleetManager { get; private set; }
        public RealmManager RealmManager { get; private set; }
        internal EventManager EventManager { get; private set; }
        public ProvinceManager ProvinceManager { get; private set; }
        public Calendar.Calendar Calendar { get; private set; }
        public Vector2I Size { get; set; }

        private event CustomEventHandler<RealmToken> NewRealm;
        private bool _tokenAcquired = false;
        private readonly DataLock _datalock;
        
        public IEnumerable<Notification> GetLiveNotifications(RealmToken realm)
        {
            List<Notification> notification = _liveNotifications.Where(n=>n.ValidForRealm(realm)).ToList();
            _liveNotifications.Clear();
            return notification;
        }

        public void SetNewRealmHandler(CustomEventHandler<RealmToken> function )
        {
            if (!_tokenAcquired)
            {
                NewRealm += function;
                _tokenAcquired = true;
            }
            else
                throw new InvalidOperationException("The NewRealm handler has already been acquired.");
            foreach (Realm realm in RealmManager.Realms)
                NewRealm.Invoke(new RealmToken(realm));
        }

        internal World(List<Notification> dailyNotifications, List<Notification> liveNotifications, DataLock dataLock)
        {
            _dailyNotifications = dailyNotifications;
            _liveNotifications = liveNotifications;
            _datalock = dataLock;
            EventManager = new EventManager(this);
            RealmManager = new RealmManager(this);
            FleetManager = new FleetManager(this);
            ProvinceManager = new ProvinceManager(this);
            Calendar = new Calendar.Calendar(this);
            Managers = new List<Manager> { Calendar, FleetManager, RealmManager, ProvinceManager };
            Managers.ForEach(m=>m.Initialize());
        }

        public void RegisterCommand(RealmToken source, Command command)
        {
            command.Source = source;
            if (_interactiveModeRealms.Contains(source))
            {
                DebugConsole.WriteLine("Interactive command : " + command);
                new Thread(()=>_datalock.ImmediateWrite(() =>ExecuteCommand(command))).Start();
                DebugConsole.WriteLine("Interactive command done : " + command);
                return;
            }
            DebugConsole.WriteLine("Defered command : "+command);
            _commands.Enqueue(command);
        }

        public void RegisterCommands(RealmToken source, IEnumerable<Command> commands)
        {
            foreach (Command command in commands)
                RegisterCommand(source, command);
        }

        private void DayUpdate()
        {
            Managers.ForEach(u => u.DayUpdate(_datalock));
        }

        private void ProcessCommands()
        {
            while (true)
            {
                Command command;
                if (!_commands.TryDequeue(out command)) return;
                _datalock.Write(() =>ExecuteCommand(command));
            }
        }

        private void ExecuteCommand(Command command)
        {
            if (command.IsValid())
                command.Execute();
        }


        void IProcessableWorld.Process()
        {
            ProcessCommands();
            DayUpdate();
        }

        public void RegisterInteractiveRealm(RealmToken token)
        {
            _interactiveModeRealms.Add(token);
        }

        internal void Notify(Notification notification)
        {
            _liveNotifications.Add(notification);
            _dailyNotifications.Add(notification);
        }
    }
}