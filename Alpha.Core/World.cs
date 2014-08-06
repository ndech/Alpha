﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Alpha.Common;
using Alpha.Core.Commands;
using Alpha.Core.Fleets;
using Alpha.Core.Province;
using Alpha.Core.Realms;

namespace Alpha.Core
{
    public class World : IProcessableWorld
    {
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private List<IManager> Managers { get; set; }
        public FleetManager FleetManager { get; private set; }
        public RealmManager RealmManager { get; private set; }
        public ProvinceManager ProvinceManager { get; private set; }

        public World()
        {
            RealmManager = new RealmManager();
            FleetManager = new FleetManager();
            ProvinceManager = new ProvinceManager();
            Managers = new List<IManager> {FleetManager, RealmManager, ProvinceManager};

        }

        public void RegisterCommand(Command command)
        {
            _commands.Enqueue(command);
        }

        public void RegisterCommands(IEnumerable<Command> commands)
        {
            foreach (Command command in commands)
                RegisterCommand(command);
        }

        private void DayUpdate(Object datalock)
        {
            Managers.ForEach(u => u.DayUpdate(datalock));
        }

        private void ProcessCommands(Object datalock)
        {
            while (true)
            {
                Command command;
                if (!_commands.TryDequeue(out command)) return;
                lock (datalock)
                {
                    if(command.IsValid())
                        command.Execute();
                }
            }
        }

        void IProcessableWorld.Process(Object dataLock)
        {
            ProcessCommands(dataLock);
            DayUpdate(dataLock);
        }
    }
}