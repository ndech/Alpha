using System;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;

namespace Alpha.Common
{
    public class ConfigurationManager
    {
        private ScreenModeEnum ScreenMode { get; } = ScreenModeEnum.Fullscreen;
        private WorldParameterEnum WorldParameter { get; } = WorldParameterEnum.Small;

        public static ConfigurationManager Config { get; } = new ConfigurationManager();
        public string Title => "Test DirectX";
        public bool AntiAliasing => PreprocessorHelper.IfDebug(false, true);
        public bool WindowedMode => ScreenMode != ScreenModeEnum.Fullscreen;
        public bool VSync => true;
        public float NearLimit => 10;
        public float FarLimit => 50000;
        public Vector2I WorldSize => WorldParameter == WorldParameterEnum.Large ? new Vector2I(4000, 2000) : new Vector2I(2000, 1000);
        public int NumberOfRegions => WorldParameter == WorldParameterEnum.Large ? 15000 : (WorldParameter == WorldParameterEnum.Medium ? 5000 : 1000);

        public Vector2I ScreenSize
        {
            get
            {
                switch (ScreenMode)
                {
                    case ScreenModeEnum.Fullscreen: return new Vector2I(1920, 1080);
                    case ScreenModeEnum.Large:      return new Vector2I(1500, 800);
                    case ScreenModeEnum.Windowed:   return new Vector2I(1024, 768);
                    default: throw new InvalidOperationException();
                }
            }
        }
        
        private enum ScreenModeEnum { Fullscreen, Windowed, Large };
        private enum WorldParameterEnum { Small, Medium, Large };
    }
}
