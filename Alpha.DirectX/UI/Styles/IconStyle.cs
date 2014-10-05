using System;
using System.Collections.Generic;
using Alpha.DirectX.UI.Controls;

namespace Alpha.DirectX.UI.Styles
{
    class IconStyle : Style<IconButton>
    {
        public String BaseTexture { get; private set; }
        public String HoveredTexture { get; private set; }
        public String ClickedTexture { get; private set; }

        internal override void Apply(IEnumerable<StyleItem> stylePartials)
        {
            foreach (StyleItem styleItem in stylePartials)
            {
                if (styleItem is TextureStyleItem)
                {
                    TextureStyleItem item = styleItem as TextureStyleItem;
                    if (item.TextureType == TextureStyleItem.Type.Base)
                        BaseTexture = item.Path;
                    else if (item.TextureType == TextureStyleItem.Type.Clicked)
                        ClickedTexture = item.Path;
                    else if (item.TextureType == TextureStyleItem.Type.Hovered)
                        HoveredTexture = item.Path;
                }
            }
        }
    }
}
