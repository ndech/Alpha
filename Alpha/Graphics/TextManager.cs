using System;
using System.Collections.Generic;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.Graphics
{
    public class TextManager : IDisposable
    {
        private readonly FontShader _fontShader;
        private readonly Dictionary<String, Font> _fontDictionary;

        private readonly Device _device;
        private Vector2I _screenSize;

        public TextManager(Device device, Vector2I screenSize)
        {
            _fontShader = new FontShader(device);
            _fontDictionary = new Dictionary<string, Font>();
            _screenSize = screenSize;
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
            return new Text(_device, _fontShader, _screenSize, _fontDictionary[fontKey], maxLength, color);
        }
    }
}
