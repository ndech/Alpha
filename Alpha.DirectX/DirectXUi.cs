﻿using System.Drawing;
using System.Windows.Forms;
using Alpha.Common;
using Alpha.Core;
using Alpha.DirectX.UI;
using Alpha.DirectX.UI.Screens;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Windows;

namespace Alpha.DirectX
{
    public class DirectXUi : IUi
    {
        private readonly Toolkit.Timer _timer;

        private RenderForm _form;
        private Dx11 _directX;
        private UiManager _uiManager;
        private Input.Input _input;
        private Camera _camera;
        private Context _context;
        private readonly IGame _game;
        private readonly WorldContainer _worldContainer;
        private readonly NotificationResolver _notificationResolver;

        public DirectXUi(IGame game, WorldContainer worldContainer)
        {
#if GPU_DEBUG
            SharpDX.Configuration.EnableObjectTracking = true;
#endif
            _game = game;
            _worldContainer = worldContainer;
            _timer = new Toolkit.Timer();
            _notificationResolver = new NotificationResolver();
        }

        private void Initialize(DataLock dataLock)
        {
            CreateWindow();
            _directX = new Dx11(_form);
            _uiManager = new UiManager();
            _input = new Input.Input();
            _camera = new Camera(new Vector3(0, 200, 0), new Vector3(0, 0.7f, 0));
            _context = new Context(_form, _directX, _game, _worldContainer, _uiManager, _input, _camera, _notificationResolver, dataLock);
            _camera.Initialize(_context);
            _input.Initialize(_context);
            _uiManager.Initialize(_context);
            _uiManager.AddScreen(new WorldParametersScreen(_context));
        }

        private void Update(double delta)
        {
            if(_worldContainer.Ready)
                _notificationResolver.Process(_worldContainer.World.GetLiveNotifications(_worldContainer.PlayerRealm));
            _camera.Update(delta);
            if(_form.Focused)
                _input.Update(delta);
            _uiManager.Update(delta);
        }

        private void Draw()
        {
            _directX.BeginScene(0.75f, 0.75f, 0.75f, 1f);
            _directX.SetAlphaBlending(true);
            _directX.SetZBuffer(false);
            _uiManager.Render(_directX.DeviceContext, _context.Camera.UiMatrix, _directX.OrthoMatrix);
            _directX.DrawScene();
        }

        public void StartRenderLoop(DataLock dataLock)
        {
            Initialize(dataLock);
            RenderLoop.Run(_form, () =>
            {
                dataLock.UiRead(()=> Update(_timer.Tick()));
                Draw();
            });
            _game.Exit();
        }

        private void CreateWindow()
        {
            _form = new RenderForm(ConfigurationManager.Config.Title)
            {
                ClientSize = new Size(ConfigurationManager.Config.ScreenSize.X, ConfigurationManager.Config.ScreenSize.Y),
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
            _form.Show();
            _form.Name = "Alpha";
        }

        public void Dispose()
        {
            _directX.Dispose();
            _context.Dispose();
            _uiManager.Dispose();
        }
    }
}
