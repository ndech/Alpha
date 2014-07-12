using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Alpha.Scripting;
using Alpha.Scripting.Providers;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    interface IFleetManager : IService, IFleetProvider
    {
        IList<Fleet> Fleets { get; set; }
    }
    class FleetManager : RenderableGameComponent, IFleetManager, IDailyUpdatable
    {
        public IList<Fleet> Fleets { get; set; }
        public FleetManager(IGame game, int updateOrder = 0) : base(game, updateOrder, true, false, true)
        {
            Fleets = new List<Fleet>();
            Fleets.Add(new Fleet{Name = "Royal fleet", ShipCount = 120});
        }

        public void RegisterAsService()
        {
            Game.Services.Register<IFleetManager>(this);
        }

        public override void Initialize(Action<string> feedback)
        {

        }

        public override void Update(double delta)
        {

        }

        public override void Dispose()
        {

        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {

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
