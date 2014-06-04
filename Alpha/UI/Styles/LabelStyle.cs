using System;
using System.Collections.Generic;
using Alpha.Graphics;
using SharpDX;

namespace Alpha.UI.Styles
{
    class LabelStyle
    {
        public VerticalAlignment VerticalAlignment { get; private set; }
        public HorizontalAlignment HorizontalAlignment { get; private set; }
        public Color TextColor { get; private set; }
        public String Font { get; set; }
        public Int32 FontSize { get; set; }

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
            }
        }
    }
}
