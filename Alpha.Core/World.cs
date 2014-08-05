using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Alpha.Common;
using Alpha.Core.Commands;
using Alpha.Core.Fleets;

namespace Alpha.Core
{
    public class World : IProcessableWorld
    {
        private readonly ConcurrentQueue<Command> _commands = new ConcurrentQueue<Command>();
        private List<IDailyUpdatable> Updatables { get; set; }
        public FleetManager FleetManager { get; private set; }

        public World()
        {
            FleetManager = new FleetManager();
            Updatables = new List<IDailyUpdatable> {FleetManager};
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
            Updatables.ForEach(u => u.DayUpdate(datalock));
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

        void IProcessableWorld.Process(object dataLock)
        {
            ProcessCommands(dataLock);
            DayUpdate(dataLock);
        }
    }
}