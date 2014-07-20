using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Scripting;
using Alpha.Scripting.Providers;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    interface IFleetManager : IService, IFleetProvider
    {
        IList<Fleet> Fleets { get; set; }
        void CreateFleet();
        IEnumerable<FleetMoveOrder> Moves { get; }
        void RegisterMove(FleetMoveOrder move);
    }
    class FleetManager : RenderableGameComponent, IFleetManager, IDailyUpdatable
    {
        public IList<Fleet> Fleets { get; set; }
        public IList<FleetMoveOrder> Moves { get; set; }
        IEnumerable<FleetMoveOrder> IFleetManager.Moves { get { return Moves; } }
        public void RegisterMove(FleetMoveOrder move)
        {
            Moves = Moves.Where(m => m.Fleet != move.Fleet).ToList();
            if(move.Steps.Count > 0)
                Moves.Add(move);
        }

        private IWorld _world;
        private FleetRenderer _fleetRenderer;
        public void CreateFleet()
        {
            Fleets.Add(new Fleet { Name = "Reinforcements", ShipCount = 15, Location = _world.Sites.Where(s => s.IsWater).RandomItem(), Speed = 2 });
        }


        public FleetManager(IGame game) : base(game, 0, true, true)
        {
            RequiredForStartUp = false;
            Fleets = new List<Fleet>();
            Moves = new List<FleetMoveOrder>();
        }
        
        public void RegisterAsService()
        {
            Game.Services.Register<IFleetManager>(this);
        }

        public override void Initialize(Action<string> feedback)
        {
            _fleetRenderer = new FleetRenderer(Game);
            _world = Game.Services.Get<IWorld>();
            Fleets.Add(new Fleet { Name = "Royal fleet", ShipCount = 120, Location = _world.Sites.Where(s=>s.IsWater).RandomItem(), Speed=2 });
        }

        public override void Update(double delta)
        {
            foreach (FleetMoveOrder move in Moves)
            {
                move.Update(delta);
            }
        }

        public override void Dispose()
        {

        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _fleetRenderer.Render(Fleets, deviceContext, viewMatrix, projectionMatrix);
            foreach (FleetMoveOrder moveOrder in Moves)
                moveOrder.Render(deviceContext, viewMatrix, projectionMatrix);

        }

        public void DayUpdate()
        {
            foreach (FleetMoveOrder move in Moves)
            {
                move.DayUpdate();
            }
        }

        IList<IScriptableFleet> IFleetProvider.ScripableFleets
        {
            get { return Fleets.Cast<IScriptableFleet>().ToList(); }
        }
    }
}
