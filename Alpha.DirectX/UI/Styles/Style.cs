using System.Collections.Generic;
using Alpha.DirectX.UI.Controls;

namespace Alpha.DirectX.UI.Styles
{
    abstract class Style<T> where T : Control
    {
        abstract internal void Apply(IEnumerable<StyleItem> items);
    }
}