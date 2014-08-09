using System;
using System.Collections.Generic;
using System.Threading;
using Alpha.Core.Commands;
using Alpha.Core.Realms;
using Alpha.Toolkit;

namespace Alpha.AI
{
    public class Ai : IAi
    {
        private readonly Realm _realm;

        public Ai(Realm realm)
        {
            _realm = realm;
        }

        public IList<Command> Process()
        {
            Thread.Sleep(RandomGenerator.Get(100, 1000));
            Console.WriteLine("IA calculations done for realm " + _realm);
            return new List<Command>();
        }
    }
}
