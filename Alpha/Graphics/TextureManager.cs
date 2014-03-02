using System;
using System.Collections.Generic;
using SharpDX.Direct3D11;

namespace Alpha.Graphics
{
    public class TextureManager : IDisposable
    {
        private readonly Dictionary<String, Texture> _textureDictionary;
        private readonly Device _device;

        public TextureManager(Device device)
        {
            _device = device;
            _textureDictionary = new Dictionary<string, Texture>();
        }

        public void Dispose()
        {
            foreach (KeyValuePair<String, Texture> keyValuePair in _textureDictionary)
                keyValuePair.Value.Dispose();
        }

        public Texture Create(String fileName, String path = "Data/Textures/")
        {
            String key = path + fileName;
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
