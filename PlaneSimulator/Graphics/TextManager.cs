using System;
using System.Collections.Generic;
using PlaneSimulator.Graphics.Shaders;
using PlaneSimulator.Toolkit;
using SharpDX;
using SharpDX.Direct3D11;

namespace PlaneSimulator.Graphics
{
    class TextManager : IDisposable
    {
        private readonly FontShader _fontShader;
        private readonly Dictionary<String, Font> _fontDictionary;

        private readonly Device _device;

        private int _screenWidth;
        private int _screenHeight;

        public TextManager(Device device, int screenWidth, int screenHeight)
        {
            _fontShader = new FontShader(device);
            _fontDictionary = new Dictionary<string, Font>();
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _device = device;
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_fontShader);
            foreach (KeyValuePair<string, Font> keyValuePair in _fontDictionary)
                keyValuePair.Value.Dispose();
        }

        public Text Create(String font, int size, int maxLength, Vector4 color)
        {
            String fontKey = font + "-" + size;
            if(!_fontDictionary.ContainsKey(fontKey))
                _fontDictionary.Add(fontKey, new Font(_device, font, size));
            return new Text(_device, _fontShader, _screenWidth, _screenHeight, _fontDictionary[fontKey], maxLength, color);
        }
    }
}
