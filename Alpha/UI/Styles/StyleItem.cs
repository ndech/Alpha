using System;
using System.Reflection;
using System.Xml.Linq;
using Alpha.Graphics;
using SharpDX;

namespace Alpha.UI.Styles
{
    class StyleItem
    {
        public static StyleItem Create(XElement styleItem)
        {
            if(styleItem.Name.LocalName.Equals("horizontalAlignment"))
                return new HorizontalAlignmentStyleItem(styleItem.Value);
            else if (styleItem.Name.LocalName.Equals("verticalAlignment"))
                return new VerticalAlignmentStyleItem(styleItem.Value);
            else if (styleItem.Name.LocalName.Equals("textColor"))
                return new TextColorStyleItem(styleItem.Value);
            else
                throw new InvalidOperationException();
        }
    }

    class HorizontalAlignmentStyleItem : StyleItem
    {
        private readonly HorizontalAlignment _horizontalAlignment;
        public HorizontalAlignment HorizontalAlignment { get { return _horizontalAlignment; } }

        public HorizontalAlignmentStyleItem(string name)
        {
            HorizontalAlignment.TryParse(name, true, out _horizontalAlignment);
        }
    }
    class VerticalAlignmentStyleItem : StyleItem
    {
        private readonly VerticalAlignment _verticalAlignment;
        public VerticalAlignment VerticalAlignment { get { return _verticalAlignment; } }

        public VerticalAlignmentStyleItem(string name)
        {
            VerticalAlignment.TryParse(name, true, out _verticalAlignment);
        }
    }
    class TextColorStyleItem : StyleItem
    {
        private readonly Color _textColor;
        public Color TextColor { get { return _textColor; } }

        public TextColorStyleItem(string name)
        {
            var property = _textColor.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);
            if(property != null)
                _textColor = (Color)property.GetValue(null);
        }
    }
}
