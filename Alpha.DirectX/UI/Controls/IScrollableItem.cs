using System;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI.Controls
{
    interface IScrollableItem<in T>
    {
        void Set(T item);
        Vector2I ItemSize();
    }
}
