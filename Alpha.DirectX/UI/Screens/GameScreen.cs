﻿using System.Linq;
using Alpha.Core.Commands;
using Alpha.DirectX.UI.Controls;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.World;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI.Screens
{
    class GameScreen : Screen
    {
        private readonly Sun _sun;
        private readonly Water _water;
        private readonly Sky _sky;
        private readonly FleetRenderer _fleetRenderer;
        private readonly FpsCounter _counter;
        public GameScreen(IContext context) : base(context, "game_screen", false)
        {
            _sun = new Sun();
            _water = new Water(context, context.World.ProvinceManager.SeaProvinces);
            _sky = new Sky(context);
            _fleetRenderer = new FleetRenderer(context);
            _counter = new FpsCounter();
            Register(new DynamicLabel(context, "calendar", new UniRectangle(new UniScalar(1.0f, -100), 0, 100, 50),
                () => context.World.Calendar.CurrentDate.ToString()));
            Register(new DynamicLabel(context, "fps", new UniRectangle(100, 0, 100, 50),
                () => _counter.Value + "FPS"));
            Register(new DynamicLabel(context, "treasury", new UniRectangle(new UniScalar(0.5f, -50), 0, 100, 50),
                () => context.Realm.Economy.Treasury.ToString()));
            Button button;
            Register(button = new Button(context, "boost_treasury", new UniRectangle(new UniScalar(0.5f, -50), 50, 100, 50),
                "Click"));
            button.Clicked += (b) => context.RegisterCommand(new ChangeTreasuryCommand(context.Realm, 10));
        }

        protected override void Update(double delta)
        {
            _water.Update(delta);
            _counter.Update(delta);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _sky.Render(deviceContext, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun, Context.Camera);
            _water.Render(deviceContext, Matrix.Identity, Context.Camera.ViewMatrix, Context.DirectX.ProjectionMatrix, _sun);
            _fleetRenderer.Render(deviceContext, Context.Camera.ViewMatrix,
                Context.DirectX.ProjectionMatrix, _sun, Context.Camera);
        }
    }
}