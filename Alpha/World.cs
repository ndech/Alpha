using System;
using System.Collections.Generic;
using Alpha.Graphics;
using Alpha.WorldGeneration;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha
{
    interface IWorld : IService
    {
        Sun Sun { get; }
        List<VoronoiSite> Sites { get; }
    }
    class World : RenderableGameComponent, IWorld
    {
        public Sun Sun { get; private set; }
        private Terrain _terrain;
        private Sky _sky;
        private Water _water;
        private IRenderer _renderer;
        private ICamera _camera;
        public List<VoronoiSite> Sites { get; set; }

        public World(IGame game) : base(game, -1)
        {
            RequiredForStartUp = false;
            Sun = new Sun();
        }

        public void RegisterAsService()
        {
            Game.Services.Register<IWorld>(this);
        }

        public override void Initialize(Action<string> feedback)
        {
            feedback.Invoke("Generating world ...");
            Sites = Generator.Create(2000, 1000, 1000, 1, 1256);
            _renderer = Game.Services.Get<IRenderer>();
            _camera = Game.Services.Get<ICamera>();
            _sky = new Sky(_renderer);
            feedback.Invoke("Drawing terrain ...");
            _terrain = new Terrain(_renderer, Sites);
            feedback.Invoke("Drawing water ...");
            _water = new Water(_renderer, Sites);
        }

        public override void Update(double delta)
        {
            _sky.Update(delta);
            _water.Update(delta);
        }

        public override void Dispose()
        { }

        public override void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _sky.Render(_renderer, deviceContext, viewMatrix, projectionMatrix, Sun, _camera);
            _water.Render(_renderer, deviceContext, Matrix.Identity, viewMatrix, projectionMatrix, Sun);
            _terrain.Render(_renderer, deviceContext, Matrix.Identity, viewMatrix, projectionMatrix, Sun);
        }
    }
}
