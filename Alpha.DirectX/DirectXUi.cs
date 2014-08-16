using System.Drawing;
using System.Windows.Forms;
using Alpha.Common;
using Alpha.Core;
using Alpha.DirectX.UI;
using Alpha.DirectX.UI.Screens;
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
        private readonly WorldContainer _worldContainer;

        public DirectXUi(IGame game, WorldContainer worldContainer)
        {
            _game = game;
            _worldContainer = worldContainer;
            _timer = new Toolkit.Timer();
        }

        private void Initialize()
        {
            CreateWindow();
            _directX = new Dx11(_form);
            _uiManager = new UiManager();
            _input = new Input.Input();
            _context = new Context(_form, _directX, _game, _worldContainer, _uiManager, _input);
            _input.Initialize(_context);
            _uiManager.Initialize(_context);
            _uiManager.AddScreen(new WorldParametersScreen(_context));
        }

        private void Update(double delta)
        {
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
