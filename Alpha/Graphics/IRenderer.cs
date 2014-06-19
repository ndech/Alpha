using System;
using Alpha.Graphics.Shaders;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.Windows;

namespace Alpha.Graphics
{
    interface IRenderer : IService
    {
        RenderForm Form { get; }
        TextManager TextManager { get; }
        ColorShader ColorShader { get; }
        TextureShader TextureShader { get; }
        WaterShader WaterShader { get; }
        TerrainShader TerrainShader { get; }
        LightShader LightShader { get; }
        FontShader FontShader { get; }
        TextureManager TextureManager { get; }
        Device Device { get; }
        Vector2I ScreenSize { get; }
        String VideoCardName { get; }
        Int32 VideoCardMemorySize { get; }
        DepthStencilView RenderToTextureDepthStencilView { get; }
        Matrix ProjectionMatrix { get; }
        void SetBackBufferAsRenderTarget();
        void EnableZBuffer();
        void DisableZBuffer();
    }
}
