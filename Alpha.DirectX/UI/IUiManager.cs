using System.Windows.Input;
using Alpha.DirectX.UI.Controls;
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
        Vector2I RelativeMousePosition(Vector2I origin);
        Vector2I PreviousMousePosition { get; }
        bool IsKeyPressed(Key key);
        void RecalculateActiveComponents();
        bool IsAnyKeyPressed(params Key[] keys);
        void SetScreen(Screen screen);
        void SetMousePointer(MousePointer.CursorType type);
        Tooltip VisibleTooltip { set; }
    }
}