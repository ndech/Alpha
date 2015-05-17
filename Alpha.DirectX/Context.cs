using Alpha.Common;
using Alpha.Core;
using Alpha.Core.Commands;
using Alpha.Core.Realms;
using Alpha.DirectX.Input;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI;
using Alpha.DirectX.UI.Text;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX.Windows;

namespace Alpha.DirectX
{
    class Context : IContext
    {
        private readonly WorldContainer _worldContainer;
        public IUiManager UiManager { get; }
        public Vector2I ScreenSize => ConfigurationManager.Config.ScreenSize;
        public RenderForm Form { get; }
        public Dx11 DirectX { get; }
        public IInput Input { get; }
        public TextManager TextManager { get; }
        public TextureManager TextureManager { get; }
        public ShaderManager Shaders { get; }
        public ICamera Camera { get; }
        public IGame Game { get; }
        public World World => _worldContainer.World;
        public Realm Realm => _worldContainer.PlayerRealm;
        public RealmToken RealmToken => _worldContainer.PlayerRealm;
        public NotificationResolver NotificationResolver { get; }
        public DataLock DataLock { get; }

        public void RegisterCommand(Command command)
        {
            World.RegisterCommand(RealmToken, command);
        }

        public Context(RenderForm form, Dx11 directX, IGame game, WorldContainer worldContainer, IUiManager uiManager, IInput input, Camera camera, NotificationResolver notificationResolver, DataLock datalock)
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
            DataLock = datalock;
        }

        public void Dispose()
        {
            Shaders.Dispose();
            TextureManager.Dispose();
        }
    }
}
