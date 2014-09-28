using Alpha.Common;
using Alpha.Core;
using Alpha.Core.Commands;
using Alpha.Core.Realms;
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
        private readonly WorldContainer _worldContainer;
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
        public World World { get { return _worldContainer.World; } }
        public Realm Realm { get { return _worldContainer.PlayerRealm; } }
        public RealmToken RealmToken { get { return _worldContainer.PlayerRealm; } }
        public NotificationResolver NotificationResolver { get; private set; }

        public void RegisterCommand(Command command)
        {
            World.RegisterCommand(RealmToken, command);
        }

        public Context(RenderForm form, Dx11 directX, IGame game, WorldContainer worldContainer, IUiManager uiManager, IInput input, Camera camera, NotificationResolver notificationResolver)
        {
            _worldContainer = worldContainer;
            NotificationResolver = notificationResolver;
            Form = form;
            DirectX = directX;
            Game = game;
            TextureManager = new TextureManager(DirectX.Device);
            TextManager = new TextManager(this);
            Shaders = new ShaderManager(DirectX.Device);
            Camera = camera;
            UiManager = uiManager;
            Input = input;
        }
    }
}
