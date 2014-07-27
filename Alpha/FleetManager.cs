using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Graphics;
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
        void SetMoveOrder(Fleet fleet, List<MoveOrder.Step> steps);

    }
    class FleetManager : RenderableGameComponent, IFleetManager, IDailyUpdatable
    {
        public IList<Fleet> Fleets { get; set; }

        private FleetRenderer _fleetRenderer;
        private IProvinceManager _provinceManager;
        public void CreateFleet()
        {
            Fleets.Add(new Fleet { Name = "Reinforcements", ShipCount = 15, Location = _provinceManager.SeaProvinces.RandomItem(), Speed = 2 });
        }
        
        public void SetMoveOrder(Fleet fleet, List<MoveOrder.Step> steps)
        {
            fleet.MoveOrder = steps == null ? null : new FleetMoveOrder(Game.Services.Get<IRenderer>(), fleet, steps);
        }

        public FleetManager(IGame game)
            : base(game, 0, true, true)
        {
            RequiredForStartUp = false;
            Fleets = new List<Fleet>();
        }

        public void RegisterAsService()
        {
            Game.Services.Register<IFleetManager>(this);
        }

        public override void Initialize(Action<string> feedback)
        {
            _fleetRenderer = new FleetRenderer(Game);
            _provinceManager = Game.Services.Get<IProvinceManager>();
            Fleets.Add(new Fleet { Name = "Royal fleet", ShipCount = 120, Location = _provinceManager.SeaProvinces.RandomItem(), Speed = 2 });
        }

        public override void Update(double delta)
        {
            foreach (Fleet fleet in Fleets)
                fleet.Update(delta);
        }

        public override void Dispose()
        { }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _fleetRenderer.Render(Fleets, deviceContext, viewMatrix, projectionMatrix);
        }

        public void DayUpdate()
        {
            foreach (Fleet fleet in Fleets)
                fleet.DayUpdate();
        }

        IList<IScriptableFleet> IFleetProvider.ScripableFleets
        {
            get { return Fleets.Cast<IScriptableFleet>().ToList(); }
        }
    }
}