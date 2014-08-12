using System.Windows.Input;
using Alpha.DirectX.UI.Screens;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI
{
    interface IUiManager
    {
        void AddScreen(Screen screen);
        void DeleteScreen(Screen screen);
        Vector2I ScreenSize { get; }
        StyleManager StyleManager { get; }
        Vector2I MousePosition { get; }
        bool IsKeyPressed(Key key);
        void RecalculateActiveComponents();
        bool IsAnyKeyPressed(params Key[] keys);
    }
}