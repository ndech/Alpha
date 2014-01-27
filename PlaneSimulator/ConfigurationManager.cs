namespace PlaneSimulator
{
    using System;
    class ConfigurationManager
    {
        private ConfigurationManager()
        {
            WindowedMode = true;
            VSync = false;
            AntiAliasing = true;
            Height = 768;
            Width = 1024;
            FarLimit = 50000.0f;
            NearLimit = 10f;
            DisplayOverlay = true;
        }

        private static readonly ConfigurationManager Instance = new ConfigurationManager();
        public static ConfigurationManager Config { get { return Instance; } }
        public String Title { get { return "Test DirectX"; } }
        public bool AntiAliasing { get; set; }
        public bool WindowedMode { get; set; }
        public bool VSync { get; set; }
        public bool DisplayOverlay { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float NearLimit { get; set; }
        public float FarLimit { get; set; }
    }
}
