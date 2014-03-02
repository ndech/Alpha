using System;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.Graphics
{
    class RenderTexture : IDisposable
    {
        private readonly Texture2D _renderTargetTexture;
        private readonly RenderTargetView _renderTargetView;
        public ShaderResourceView ShaderResourceView { get; private set; }
        public RenderTexture(Device device, Vector2I screenSize)
        {
            var textureDesc = new Texture2DDescription()
            {
                Width = screenSize.X,
                Height = screenSize.Y,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R32G32B32A32_Float,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            _renderTargetTexture = new Texture2D(device, textureDesc);

            _renderTargetView = new RenderTargetView(device, _renderTargetTexture, 
                new RenderTargetViewDescription
                {
                    Format = textureDesc.Format,
                    Dimension = RenderTargetViewDimension.Texture2D,
                    Texture2D = {MipSlice = 0},
                });

            // Create the render target view.
            ShaderResourceView = new ShaderResourceView(device, _renderTargetTexture, 
                new ShaderResourceViewDescription
                {
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = { MipLevels = 1, MostDetailedMip = 0 },
                });
        }

        public void Dispose()
        {
            ShaderResourceView.Dispose();
            _renderTargetView.Dispose();
            _renderTargetTexture.Dispose();
        }

        public void SetRenderTarget(DeviceContext context, DepthStencilView depthStencilView)
        {
            context.OutputMerger.SetTargets(depthStencilView, _renderTargetView);
        }

        public void ClearRenderTarget(DeviceContext context, DepthStencilView depthStencilView, float red, float green, float blue, float alpha)
        {
            context.ClearRenderTargetView(_renderTargetView, new Color4(red, green, blue, alpha));
            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }
    }
}
