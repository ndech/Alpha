using System;
using System.Collections.Generic;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.Shaders
{
    class ShaderManager : IDisposable
    {
        private Dictionary<Type, Shader> _shaders; 

        public ShaderManager(Device device)
        {
            _shaders = new Dictionary<Type, Shader>();
            Register(new ColorShader(device)
                    ,new FontShader(device)
                    ,new TextureShader(device)
                    ,new WaterShader(device)
                    ,new LightShader(device)
                    ,new PathShader(device)
                    ,new TerrainShader(device)
                    ,new WorldTerrainShader(device)
                    ,new Texture1DShader(device));
        }

        private void Register(params Shader[] shaders)
        {
            foreach (Shader shader in shaders)
                _shaders.Add(shader.GetType(), shader);
        }

        public void Dispose()
        {
            foreach (Shader shader in _shaders.Values)
                shader.Dispose();
        }

        public T Get<T>() where T : Shader
        {
            return (T)_shaders[typeof (T)];
        }
    }
}
