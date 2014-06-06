using System;
using System.Reflection;
using System.Xml.Linq;
using Alpha.Graphics;
using Alpha.Toolkit;
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
            else if (styleItem.Name.LocalName.Equals("font"))
                return new FontStyleItem(styleItem.Value);
            else if (styleItem.Name.LocalName.Equals("fontSize"))
                return new FontSizeStyleItem(styleItem.Value);
            else if (styleItem.Name.LocalName.Equals("padding"))
                return new PaddingStyleItem(styleItem.Value);
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
        public Color TextColor { get; private set; }

        public TextColorStyleItem(string value)
        {
            TextColor = ColorParser.Parse(value);
        }
    }
    class FontStyleItem : StyleItem
    {
        public String Font { get; private set; }

        public FontStyleItem(string name)
        {
            Font = name;
        }
    }
    class FontSizeStyleItem : StyleItem
    {
        public Int32 FontSize { get; private set; }

        public FontSizeStyleItem(string name)
        {
            FontSize = Int32.Parse(name);
        }
    }
    class PaddingStyleItem : StyleItem
    {
        public Padding Padding { get; private set; }

        public PaddingStyleItem(string name)
        {
            Padding = new Padding(Int32.Parse(name));
        }
    }
}
