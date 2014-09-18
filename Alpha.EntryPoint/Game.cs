using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alpha.AI;
using Alpha.Common;
using Alpha.Core;
using Alpha.Core.Notifications;
using Alpha.DirectX;
using Alpha.Toolkit;

namespace Alpha.EntryPoint
{
    public class Game : IGame
    {
        private readonly IUi _ui;
        private readonly IList<IAi> _ais;
        private readonly List<RealmToken> _tokens = new List<RealmToken>();
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
            Thread.CurrentThread.Name = "Main thread";
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
            DebugConsole.WriteLine("Generation begin");
            _world = (World)((IWorldGenerator)(new WorldGenerator(_dailyNotifications, _liveNotifications, dataLock)))
                .Generate(s => _loadingMessage = s);
            DebugConsole.WriteLine("Generation is done");
            _world.SetNewRealmHandler(NewRealm);
            //Choose one of the realms as the UI realm
            _worldContainer.PlayerRealm = _tokens.First();
            _ais.Remove(_ais.Single(ai => ai.Realm == _tokens.First().Realm));
            _world.RegisterInteractiveRealm(_tokens.First());
            _worldContainer.World = _world;
            _isWorldGenerationDone = true;
            _dayTimer.Start();
            while (_continue)
            {
                _dailyNotifications.Clear();
                ((IProcessableWorld)_world).Process();
                Parallel.ForEach(_ais, 
                    new ParallelOptions { MaxDegreeOfParallelism = 4 },
                    ai => _world.RegisterCommands(ai.RealmToken, ai.Process(dataLock, _dailyNotifications)));
                DebugConsole.WriteLine("Computations done");
                _dayTimer.WaitForNextDay(_continue);
                DebugConsole.WriteLine("Day end");
            }
        }

        private void NewRealm(RealmToken token)
        {
            _tokens.Add(token);
            _ais.Add(new Ai(token, _world));
        }

        public void Exit()
        {
            _continue.Stop();
        }
    }
}
