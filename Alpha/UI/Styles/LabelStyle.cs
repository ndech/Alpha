using System.Collections.Generic;
using Alpha.Graphics;
using SharpDX;

namespace Alpha.UI.Styles
{
    class LabelStyle
    {
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Color TextColor { get; set; }

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
            }
        }
    }
}
