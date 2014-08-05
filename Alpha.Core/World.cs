using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Alpha.Core.Commands;
using Alpha.Core.Fleets;
using Alpha.Toolkit;

namespace Alpha.Core
{
    public class World
    {
        private readonly ConcurrentQueue<ICommand> _commands = new ConcurrentQueue<ICommand>();
        public IFleetManager Fleets { get; private set; }

        public void RegisterCommand(ICommand command)
        {
            _commands.Enqueue(command);
        }

        public void RegisterCommands(IEnumerable<ICommand> commands)
        {
            foreach (ICommand command in commands)
                RegisterCommand(command);
        }

        private void DayUpdate()
        {
            
        }

        public void Process(object dataLock)
        {
            for (int i = 0; i < 10; i++)
            {
                lock (dataLock)
                {
                    Console.WriteLine("Processing command " + i);
                    Thread.Sleep(RandomGenerator.Get(100, 300));
                }
            }
            for (int i = 0; i < 15; i++)
            {
                lock (dataLock)
                {
                    Console.WriteLine("Processing data " + i);
                    Thread.Sleep(RandomGenerator.Get(100, 300));
                }
            }
        }
    }
}