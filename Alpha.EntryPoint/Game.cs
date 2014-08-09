using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alpha.AI;
using Alpha.Common;
using Alpha.Core;
using Alpha.Core.Realms;
using Alpha.UI;

namespace Alpha.EntryPoint
{
    public class Game : IGame
    {
        private readonly IUi _ui;
        private readonly IList<IAi> _ais;
        private readonly World _world;
        private readonly object _dataLock = new Object();
        private volatile bool _continue = true;
        public Game()
        {
            _world = new World();
            _ui = new DirectXUi();
            _ais = new List<IAi>();
            foreach (Realm realm in _world.RealmManager.Realms)
                _ais.Add(new Ai(realm));
        }

        public void Run()
        {
            new Thread(()=>_ui.StartRenderLoop(_dataLock)).Start();
            while (_continue)
            {
                ((IProcessableWorld)_world).Process(_dataLock);
                Parallel.ForEach(_ais, 
                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    ai => _world.RegisterCommands(ai.Process()));
            }
        }

        public void Exit()
        {
            _continue = false;
        }
    }
}
