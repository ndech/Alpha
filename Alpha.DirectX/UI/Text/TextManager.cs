using System;
using System.Collections.Generic;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Text
{
    class TextManager
    {
        private readonly Dictionary<string, Font> _fontDictionary;
        private readonly IContext _context;

        public TextManager(IContext context)
        {
            _fontDictionary = new Dictionary<string, Font>();
            _context = context;
        }

        public SimpleText Create(string font, int size, int maxLength, Color color)
        {
            string fontKey = font + "-" + size;
            if(!_fontDictionary.ContainsKey(fontKey))
                _fontDictionary.Add(fontKey, new Font(_context, font, size));
            return new SimpleText(_context, _fontDictionary[fontKey], maxLength, color);
        }

        public Text Create(string fontName, int fontSize, string content, Vector2I size, Color color, 
            HorizontalAlignment horizontalAligment, VerticalAlignment verticalAlignment, Padding padding)
        {
            string fontKey = fontName + "-" + size;
            if (!_fontDictionary.ContainsKey(fontKey))
                _fontDictionary.Add(fontKey, new Font(_context, fontName, fontSize));
            return new Text(_context, content, _fontDictionary[fontKey], size, color, horizontalAligment, verticalAlignment, padding);
        }
    }
}
