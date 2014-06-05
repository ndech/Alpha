using System;
using System.Collections.Generic;
using Alpha.Graphics;
using SharpDX;

namespace Alpha.UI.Styles
{
    class ButtonStyle
    {
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Color TextColor { get; set; }
        public String Font { get; set; }
        public Int32 FontSize { get; set; }
        public Padding Padding { get; set; }

        public void Apply(List<StyleItem> stylePartials)
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
