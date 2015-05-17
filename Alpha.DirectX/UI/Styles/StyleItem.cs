using System;
using System.Xml.Linq;
using Alpha.Toolkit;
using SharpDX;

namespace Alpha.DirectX.UI.Styles
{
    class StyleItem
    {
        public static StyleItem Create(XElement styleItem)
        {
            if(styleItem.Name.LocalName.Equals("horizontalAlignment"))
                return new HorizontalAlignmentStyleItem(styleItem.Value);
            if (styleItem.Name.LocalName.Equals("verticalAlignment"))
                return new VerticalAlignmentStyleItem(styleItem.Value);
            if (styleItem.Name.LocalName.Equals("textColor"))
                return new TextColorStyleItem(styleItem.Value);
            if (styleItem.Name.LocalName.Equals("font"))
                return new FontStyleItem(styleItem.Value);
            if (styleItem.Name.LocalName.Equals("fontSize"))
                return new FontSizeStyleItem(styleItem.Value);
            if (styleItem.Name.LocalName.Equals("padding"))
                return new PaddingStyleItem(styleItem.Value);
            if (styleItem.Name.LocalName.Equals("baseTexture"))
                return new TextureStyleItem(TextureStyleItem.Type.Base, styleItem.Value);
            if (styleItem.Name.LocalName.Equals("hoveredTexture"))
                return new TextureStyleItem(TextureStyleItem.Type.Hovered, styleItem.Value);
            if (styleItem.Name.LocalName.Equals("clickedTexture"))
                return new TextureStyleItem(TextureStyleItem.Type.Clicked, styleItem.Value);
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
        public string Font { get; private set; }

        public FontStyleItem(string name)
        {
            Font = name;
        }
    }
    class FontSizeStyleItem : StyleItem
    {
        public int FontSize { get; private set; }

        public FontSizeStyleItem(string name)
        {
            FontSize = int.Parse(name);
        }
    }
    class PaddingStyleItem : StyleItem
    {
        public Padding Padding { get; private set; }

        public PaddingStyleItem(string name)
        {
            Padding = new Padding(int.Parse(name));
        }
    }

    class TextureStyleItem : StyleItem
    {
        public enum Type
        {
            Base,
            Hovered,
            Clicked
        }
        public Type TextureType { get; private set; }
        public string Path { get; private set; }

        public TextureStyleItem(Type type, string path)
        {
            TextureType = type;
            Path = path;
        }
    }
}
