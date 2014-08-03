using System;
using System.Collections.Generic;
using System.Threading;
using Alpha.Core.Commands;
using Alpha.Toolkit;

namespace Alpha.AI
{
    public class Ai : IAi
    {
        private readonly int _id;

        public Ai(int id)
        {
            _id = id;
        }

        public IList<ICommand> Process()
        {
            Thread.Sleep(RandomGenerator.Get(100, 1000));
            Console.WriteLine("IA calculations done for realm " + _id);
            return new List<ICommand>();
        }
    }
}
