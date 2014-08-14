using Alpha.Common;
using Alpha.DirectX.Input;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI;
using Alpha.DirectX.UI.Text;
using Alpha.Toolkit.Math;
using SharpDX.Windows;

namespace Alpha.DirectX
{
    class Context : IContext
    {
        public IUiManager UiManager { get; private set; }
        public Vector2I ScreenSize { get { return new Vector2I(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height); } }
        public RenderForm Form { get; private set; }
        public Dx11 DirectX { get; private set; }
        public IInput Input { get; private set; }
        public TextManager TextManager { get; private set; }
        public TextureManager TextureManager { get; private set; }
        public ShaderManager Shaders { get; private set; }
        public ICamera Camera { get; private set; }
        public IGame Game { get; private set; }

        public Context(RenderForm form, Dx11 directX, IGame game)
        {
            Form = form;
            DirectX = directX;
            Game = game;
            TextureManager = new TextureManager(DirectX.Device);
            TextManager = new TextManager(this);
            Shaders = new ShaderManager(DirectX.Device);
            Camera = new Camera();
        }

        public void Initialize(IUiManager manager, IInput input)
        {
            UiManager = manager;
            Input = input;
        }
    }
}
