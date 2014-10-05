using System;
using Alpha.DirectX.UI.Controls;

namespace Alpha.DirectX.UI.Styles
{
    interface IStylable<T, TU> where TU : Style<T>, new() where T : Control
    {
        String ComponentType { get; }
        UiComponent Component { get; }
    }
}
