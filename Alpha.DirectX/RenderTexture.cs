using System;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace Alpha.DirectX
{
    class RenderTexture : IDisposable
    {
        private readonly Texture2D _renderTargetTexture;
        private readonly RenderTargetView _renderTargetView;
        private DepthStencilView _depthStencilView;
        public ShaderResourceView ShaderResourceView { get; }
        public RenderTexture(Device device, Vector2I size)
        {
            var textureDesc = new Texture2DDescription
            {
                Width = size.X,
                Height = size.Y,
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
                    Texture2D = { MipSlice = 0 },
                });

            // Create the render target view.
            ShaderResourceView = new ShaderResourceView(device, _renderTargetTexture,
                new ShaderResourceViewDescription
                {
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = { MipLevels = 1, MostDetailedMip = 0 },
                });

            _depthStencilView = new DepthStencilView(device, new Texture2D(device, new Texture2DDescription
            {
                Width = size.X,
                Height = size.Y,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }), new DepthStencilViewDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 }
            });
        }

        public void Dispose()
        {
            ShaderResourceView.Dispose();
            _renderTargetView.Dispose();
            _renderTargetTexture.Dispose();
        }

        public void SetRenderTarget(DeviceContext context)
        {
            context.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
        }

        public void ClearRenderTarget(DeviceContext context, float red, float green, float blue, float alpha)
        {
            context.ClearRenderTargetView(_renderTargetView, new Color4(red, green, blue, alpha));
            context.ClearDepthStencilView(_depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }
    }
}