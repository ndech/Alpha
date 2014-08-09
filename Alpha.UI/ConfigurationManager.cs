using System;

namespace Alpha.UI
{
    class ConfigurationManager
    {
        private enum Modes
        {
            Fullscreen,
            Windowed,
            Large
        };
        private ConfigurationManager()
        {
            SetValues(Modes.Windowed);
            AntiAliasing = true;
            FarLimit = 50000.0f;
            NearLimit = 10f;
            VSync = true;
        }

        private void SetValues(Modes mode)
        {
            if (mode == Modes.Fullscreen)
            {
                WindowedMode = false;
                Height = 900;
                Width = 1600;
                
            }
            else if (mode == Modes.Windowed)
            {
                WindowedMode = true;
                Height = 768;
                Width = 1024;
            }
            else if (mode == Modes.Large)
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
    }
}
