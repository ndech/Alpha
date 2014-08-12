using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Alpha.Common;
using Alpha.DirectX.UI;
using Alpha.UI;
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
        private Context _context;
        private readonly IGame _game;
        private List<RenderableComponent> _components;

        public DirectXUi(IGame game)
        {
            _game = game;
            _timer = new Toolkit.Timer();
        }

        private void Initialize()
        {
            CreateWindow();
            _directX = new Dx11(_form);
            _context = new Context(_form, _directX);
            _uiManager = new UiManager(_context);
            _input = new Input.Input(_context);
            _context.Initialize(_uiManager, _input);
            _components = new List<RenderableComponent> { _uiManager, new MousePointer(_context) };
            _input.Initialize();
            _components.ForEach(c => c.Initialize());
        }

        private void Update(double delta)
        {
            _input.Update(delta);
            _uiManager.Update(delta);
        }

        private void Draw()
        {
            _directX.BeginScene(0.75f, 0.75f, 0.75f, 1f);
            foreach (RenderableComponent item in _components)
            {
                _directX.SetAlphaBlending(item.BlendingEnabled);
                _directX.SetWireFrameMode(item.DisplayWireframe);
                _directX.SetZBuffer(item.ZBufferEnabled);
                if (item.ZBufferEnabled)
                    item.Render(_directX.DeviceContext, _context.Camera.ViewMatrix, _directX.ProjectionMatrix);
                else
                    item.Render(_directX.DeviceContext, _context.Camera.UiMatrix, _directX.OrthoMatrix);
            }
            _directX.DrawScene();
        }

        public void StartRenderLoop(object dataLock)
        {
            Initialize();
            RenderLoop.Run(_form, () =>
            {
                lock (dataLock)
                    Update(_timer.Tick());
                Draw();
            });
            _game.Exit();
        }

        private void CreateWindow()
        {
            _form = new RenderForm(ConfigurationManager.Config.Title)
            {
                ClientSize = new Size(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height),
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
            _form.Show();
            _form.Name = "Alpha";
        }
    }
}
