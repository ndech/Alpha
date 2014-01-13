using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneSimulator
{
    class ConfigurationManager
    {
        private ConfigurationManager()
        {
            WindowedMode = false;
            VSync = true;
            AntiAliasing = true;
            Height = 900;
            Width = 1600;
            FarLimit = 100000.0f;
            NearLimit = 0.1f;
        }

        private static readonly ConfigurationManager Instance = new ConfigurationManager();
        public static ConfigurationManager Config { get { return Instance; } }
        public String Title { get { return "Test DirectX"; } }
        public bool AntiAliasing { get; set; }
        public bool WindowedMode { get; set; }
        public bool VSync { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public float NearLimit { get; set; }
        public float FarLimit { get; set; }
    }
}
