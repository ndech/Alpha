using System;
using System.Collections.Generic;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit;
using SharpDX;

namespace Alpha.Graphics
{
    class TextManager : IDisposable
    {
        private readonly FontShader _fontShader;
        private readonly Dictionary<String, Font> _fontDictionary;

        private readonly IRenderer _renderer;

        public TextManager(IRenderer renderer)
        {
            _fontShader = new FontShader(renderer.Device);
            _fontDictionary = new Dictionary<string, Font>();
            _renderer = renderer;
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndSetToNull(_fontShader);
            foreach (KeyValuePair<string, Font> keyValuePair in _fontDictionary)
                keyValuePair.Value.Dispose();
        }

        public Text Create(String font, int size, int maxLength, Color color)
        {
            String fontKey = font + "-" + size;
            if(!_fontDictionary.ContainsKey(fontKey))
                _fontDictionary.Add(fontKey, new Font(_renderer, font, size));
            return new Text(_renderer, _fontShader, _fontDictionary[fontKey], maxLength, color);
        }
    }
}
