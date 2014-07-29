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
    interface IFleetManager : IService, IFleetProvider, ISelectableManager
    {
        IList<Fleet> Fleets { get; set; }
        void CreateFleet(Realm realm);

    }
    class FleetManager : RenderableGameComponent, IFleetManager, IDailyUpdatable
    {
        public IList<Fleet> Fleets { get; set; }

        private FleetRenderer _fleetRenderer;
        private IProvinceManager _provinceManager;
        public void CreateFleet(Realm realm)
        {
            Fleets.Add(new Fleet(realm, 15, "Reinforcements", _provinceManager.SeaProvinces.RandomItem(), 2));
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
            Fleets.Add(new Fleet(Game.Services.Get<IRealmManager>().PlayerRealm, 120, "Royal fleet", _provinceManager.SeaProvinces.RandomItem(), 2));
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

        public ISelectable Select(Picker picker)
        {
            return Fleets.FirstOrDefault(fleet => Vector3.Distance(fleet.Location.Center, picker.GroundIntersection) < 15);
        }
    }
}