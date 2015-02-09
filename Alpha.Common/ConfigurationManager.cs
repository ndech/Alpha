using System;

namespace Alpha.Common
{
    public class ConfigurationManager
    {
        private enum ScreenModes
        {
            Fullscreen,
            Windowed,
            Large
        };
        private enum WorldSize
        {
            Small,
            Medium,
            Large
        };
        private ConfigurationManager()
        {
            SetValues(ScreenModes.Large);
            SetValues(WorldSize.Small);
#if DEBUG
            AntiAliasing = false;
#else
            AntiAliasing = true;
#endif
            FarLimit = 50000.0f;
            NearLimit = 10f;
            VSync = true;
        }

        private void SetValues(WorldSize worldSize)
        {
            if (worldSize == WorldSize.Small)
            {
                NumberOfRegions = 1000;
                WorldWidth = 2000;
                WorldHeight = 1000;
            }
            else if (worldSize == WorldSize.Large)
            {
                NumberOfRegions = 15000;
                WorldWidth = 4000;
                WorldHeight = 2000;
            }
            else if (worldSize == WorldSize.Medium)
            {
                NumberOfRegions = 5000;
                WorldWidth = 2000;
                WorldHeight = 1000;
            }
        }

        private void SetValues(ScreenModes screenMode)
        {
            if (screenMode == ScreenModes.Fullscreen)
            {
                WindowedMode = false;
                Height = 900;
                Width = 1600;
                
            }
            else if (screenMode == ScreenModes.Windowed)
            {
                WindowedMode = true;
                Height = 768;
                Width = 1024;
            }
            else if (screenMode == ScreenModes.Large)
            {
                WindowedMode = true;
                Height = 800;
                Width = 1500;
            }
        }

        private static readonly ConfigurationManager Instance = new ConfigurationManager();
        public static ConfigurationManager Config { get { return Instance; } }
        public String Title { get { return "Test DirectX"; } }
        public bool AntiAliasing { get; private set; }
        public bool WindowedMode { get; private set; }
        public bool VSync { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public float NearLimit { get; private set; }
        public float FarLimit { get; private set; }

        public int WorldWidth { get; private set; }
        public int WorldHeight { get; private set; }
        public int NumberOfRegions { get; private set; }

    }
}
