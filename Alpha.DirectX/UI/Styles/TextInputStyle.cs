using System;
using System.Collections.Generic;
using Alpha.DirectX.UI.Controls;
using SharpDX;

namespace Alpha.DirectX.UI.Styles
{
    class TextInputStyle : Style<TextInput>
    {
        public VerticalAlignment VerticalAlignment { get; private set; }
        public HorizontalAlignment HorizontalAlignment { get; private set; }
        public Color TextColor { get; private set; }
        public string Font { get; private set; }
        public int FontSize { get; private set; }
        public Padding Padding { get; private set; }

        internal override void Apply(IEnumerable<StyleItem> stylePartials)
        {
            foreach (StyleItem styleItem in stylePartials)
            {
                if (styleItem is VerticalAlignmentStyleItem)
                    VerticalAlignment = ((VerticalAlignmentStyleItem)styleItem).VerticalAlignment;
                else if (styleItem is HorizontalAlignmentStyleItem)
                    HorizontalAlignment = ((HorizontalAlignmentStyleItem)styleItem).HorizontalAlignment;
                else if (styleItem is TextColorStyleItem)
                    TextColor = ((TextColorStyleItem)styleItem).TextColor;
                else if (styleItem is FontStyleItem)
                    Font = ((FontStyleItem)styleItem).Font;
                else if (styleItem is FontSizeStyleItem)
                    FontSize = ((FontSizeStyleItem)styleItem).FontSize;
                else if (styleItem is PaddingStyleItem)
                    Padding = ((PaddingStyleItem)styleItem).Padding;
            }
        }
    }
}
