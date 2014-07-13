using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
using Alpha.Scripting;
using Alpha.Scripting.Providers;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    interface IFleetManager : IService, IFleetProvider
    {
        IList<Fleet> Fleets { get; set; }
        void CreateFleet();
    }
    class FleetManager : RenderableGameComponent, IFleetManager, IDailyUpdatable
    {
        public IList<Fleet> Fleets { get; set; }
        public IList<Move> Moves { get; set; }
        private IWorld _world;
        private FleetRenderer _fleetRenderer;
        public void CreateFleet()
        {
            Fleets.Add(new Fleet { Name = "Reinforcements", ShipCount = 15, Location = _world.Sites.Where(s => s.IsWater).RandomItem() });
        }

        public FleetManager(IGame game) : base(game, 0, true, true)
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
            _world = Game.Services.Get<IWorld>();
            Fleets.Add(new Fleet { Name = "Royal fleet", ShipCount = 120, Location = _world.Sites.Where(s=>s.IsWater).RandomItem() });
        }

        public override void Update(double delta)
        {

        }

        public override void Dispose()
        {

        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _fleetRenderer.Render(Fleets, deviceContext, viewMatrix, projectionMatrix);
        }

        public void DayUpdate()
        {

        }

        IList<IScriptableFleet> IFleetProvider.ScripableFleets
        {
            get { return Fleets.Cast<IScriptableFleet>().ToList(); }
        }
    }
}
