﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Alpha.AI;
using Alpha.Common;
using Alpha.Core;
using Alpha.Core.Notifications;
using Alpha.Core.Realms;
using Alpha.DirectX;
using Alpha.Toolkit;

namespace Alpha.EntryPoint
{
    public class Game : IGame
    {
        private readonly IUi _ui;
        private readonly IList<IAi> _ais;
        private World _world;
        private readonly DayTimer _dayTimer;
        private readonly ContinueFlag _continue = new ContinueFlag();
        private readonly List<Notification> _dailyNotifications = new List<Notification>();
        private readonly List<Notification> _liveNotifications = new List<Notification>();
        public AutoResetEvent GenerateWorldEvent { get; private set; }
        private volatile string _loadingMessage = "";
        public string LoadingMessage { get { return _loadingMessage; } }
        private volatile bool _isWorldGenerationDone;
        public bool IsWorldGenerationDone { get { return _isWorldGenerationDone; } }
        private readonly WorldContainer _worldContainer = new WorldContainer();

        public Game()
        {
            _ui = new DirectXUi(this, _worldContainer);
            GenerateWorldEvent = new AutoResetEvent(false);
            _dayTimer = new DayTimer();
            _ais = new List<IAi>();
        }

        public void Run()
        {
            //Create ReadWrite lock for world data access
            DataLock dataLock = new DataLock();
            //Create UI Thread
            Thread uiThread = new Thread(()=>_ui.StartRenderLoop(dataLock));
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Name = "UiThread";
            uiThread.Start();
            //Wait for the user to press on "Generate World"
            bool ready;
            do
            {
                ready = GenerateWorldEvent.WaitOne(50);
            } while (_continue && ! ready);
            if(!_continue) return;
            //Generate the world
            Console.WriteLine("Generation begin");
            _world = (World)((IWorldGenerator)(new WorldGenerator(_dailyNotifications, _liveNotifications)))
                .Generate(s => _loadingMessage = s);
            Console.WriteLine("Generation is done");
            foreach (Realm realm in _world.RealmManager.Realms)
                _ais.Add(new Ai(realm, _world));
            _worldContainer.World = _world;
            _isWorldGenerationDone = true;
            _dayTimer.Start();
            while (_continue)
            {
                _dailyNotifications.Clear();
                ((IProcessableWorld)_world).Process(dataLock);
                Parallel.ForEach(_ais, 
                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    ai => _world.RegisterCommands(ai.Process(dataLock, _dailyNotifications)));
                Console.WriteLine("Computations done");
                _dayTimer.WaitForNextDay(_continue);
                Console.WriteLine("Day end");
            }
        }

        public void Exit()
        {
            _continue.Stop();
        }
    }
}
