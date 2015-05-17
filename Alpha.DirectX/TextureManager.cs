using System;
using System.Collections.Generic;
using SharpDX.Direct3D11;

namespace Alpha.DirectX
{
    class TextureManager : IDisposable
    {
        private readonly Dictionary<string, Texture> _textureDictionary;
        private readonly Device _device;

        public TextureManager(Device device)
        {
            _device = device;
            _textureDictionary = new Dictionary<string, Texture>();
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, Texture> keyValuePair in _textureDictionary)
                keyValuePair.Value.Dispose();
        }

        public Texture Create(string fileName, string path = "Data/Textures/")
        {
            string key = path + fileName;
            if (!_textureDictionary.ContainsKey(key))
            {
                Texture newTexture = new Texture(_device, fileName, path);
                _textureDictionary.Add(key, newTexture);
                return newTexture;
            }
            return _textureDictionary[key];
        }
    }
}
