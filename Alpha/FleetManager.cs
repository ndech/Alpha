using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Alpha.Graphics;
using Alpha.Graphics.Shaders;
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
        private ObjModel _model;
        private LightShader _shader;
        private Light _light;
        private ICamera _camera;
        public FleetManager(IGame game, int updateOrder = 0) : base(game, updateOrder)
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
            IRenderer renderer = Game.Services.Get<IRenderer>();
            _shader = renderer.LightShader;
            _model = new ObjModel(renderer.Device, "BasicBoat.obj", renderer.TextureManager.Create("Metal.png"));
            _camera = Game.Services.Get<ICamera>();
            _light = Game.Services.Get<IWorld>().Sun;
        }

        public override void Update(double delta)
        {

        }

        public override void Dispose()
        {

        }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _model.Render(deviceContext);
            _shader.Render(deviceContext, _model.IndexCount, Matrix.Scaling(10, 10, 10), viewMatrix, projectionMatrix, _model.Texture, _light, _camera);
            _shader.Render(deviceContext, _model.IndexCount, Matrix.Scaling(10, 10, 10)*Matrix.Translation(100,0,0), viewMatrix, projectionMatrix, _model.Texture, _light, _camera);
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
