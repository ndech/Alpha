using Alpha.DirectX.Input;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI;
using Alpha.DirectX.UI.Text;
using Alpha.Toolkit.Math;
using SharpDX.Windows;

namespace Alpha.DirectX
{
    interface IContext
    {
        IUiManager UiManager { get; }
        Vector2I ScreenSize { get; }
        RenderForm Form { get; }
        Dx11 DirectX { get; }
        IInput Input { get; }
        TextManager TextManager { get; }
        TextureManager TextureManager { get; }
        ShaderManager Shaders { get; }
    }
}
