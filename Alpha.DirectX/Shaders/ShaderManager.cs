﻿using SharpDX.Direct3D11;

namespace Alpha.DirectX.Shaders
{
    class ShaderManager
    {
        public ColorShader ColorShader { get; private set; }
        public FontShader FontShader { get; private set; }
        public TextureShader TextureShader { get; private set; }

        public ShaderManager(Device device)
        {
            ColorShader = new ColorShader(device);
            FontShader = new FontShader(device);
            TextureShader = new TextureShader(device);
        }
    }
}