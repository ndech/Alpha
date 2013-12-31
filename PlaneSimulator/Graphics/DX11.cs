using System;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace PlaneSimulator.Graphics
{
    class Dx11
    {
        private Rational _refreshRate;
        private Device _device;
        private DeviceContext _deviceContext;
        private SwapChain _swapChain;
        private RenderTargetView _renderTargetView;
        private Texture2D _depthStencilBuffer;
        private DepthStencilState _depthStencilState;
        private DepthStencilView _depthStencilView;
        private RasterizerState _rasterState;
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        public Matrix OrthoMatrix { get; private set; }

        public void AcquireGpu(out int videoCardMemorySize, out string videoCardName)
        {
            Factory factory = new Factory();
            Adapter gpuAdapter = factory.Adapters.First();
            Output monitor = gpuAdapter.Outputs.First();

            _refreshRate = GetHighestAvailableRefreshFrequency(monitor);

            videoCardMemorySize = gpuAdapter.Description.DedicatedVideoMemory >> 10 >> 10; //Convert bits to MB
            videoCardName = gpuAdapter.Description.Description;

            monitor.Dispose();
            gpuAdapter.Dispose();
            factory.Dispose();
        }

        private static Rational GetHighestAvailableRefreshFrequency(Output monitor)
        {
            if (ConfigurationManager.Config.WindowedMode)
                return new Rational(60, 1);
            return monitor.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced).
                Where(mode =>
                        mode.Width == ConfigurationManager.Config.Width &&
                        mode.Height == ConfigurationManager.Config.Height).Last().RefreshRate;
        }

        public void CreateDeviceAndSwapChain(RenderForm form)
        {
            var desc = new SwapChainDescription
            {
                BufferCount = 1, // Set to a single back buffer (double buffering)
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, _refreshRate, Format.R8G8B8A8_UNorm),
                IsWindowed = ConfigurationManager.Config.WindowedMode,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };
            FeatureLevel[] featureLevels = {FeatureLevel.Level_11_0, FeatureLevel.Level_10_0};
            
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, featureLevels, desc, out _device, out _swapChain);
            _deviceContext = _device.ImmediateContext;

            _swapChain.GetParent<Factory>().MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);
        }

        public void InitializeBuffers()
        {
            //Create back buffer
            Texture2D backBuffer = Resource.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_device, backBuffer);
            backBuffer.Dispose();

            //Create the depth/stencil buffer
			var depthBufferDesc = new Texture2DDescription
			{
				Width = ConfigurationManager.Config.Width,
				Height = ConfigurationManager.Config.Height,
				MipLevels = 1,
				ArraySize = 1,
				Format = Format.D24_UNorm_S8_UInt,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				BindFlags = BindFlags.DepthStencil,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None
			};
			_depthStencilBuffer = new Texture2D(_device, depthBufferDesc);

            DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                // Stencil operation if pixel front-facing.
                FrontFace = new DepthStencilOperationDescription
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                },
                // Stencil operation if pixel is back-facing.
                BackFace = new DepthStencilOperationDescription
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            };

            // Create the depth stencil state.
            _depthStencilState = new DepthStencilState(_device, depthStencilDesc);

            // Set the depth stencil state.
            _deviceContext.OutputMerger.SetDepthStencilState(_depthStencilState, 1);

            // Initialize and set up the depth stencil view.
            var depthStencilViewDesc = new DepthStencilViewDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource
                {
                    MipSlice = 0
                }
            };

            // Create the depth stencil view.
            _depthStencilView = new DepthStencilView(_device, _depthStencilBuffer, depthStencilViewDesc);

            // Bind the render target view and depth stencil buffer to the output render pipeline.
            _deviceContext.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);

            // Setup the raster description which will determine how and what polygon will be drawn.
            var rasterDesc = new RasterizerStateDescription
            {
                IsAntialiasedLineEnabled = false,
                CullMode = CullMode.Back,
                DepthBias = 0,
                DepthBiasClamp = .0f,
                IsDepthClipEnabled = true,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = .0f
            };

            // Create the rasterizer state from the description we just filled out.
            _rasterState = new RasterizerState(_device, rasterDesc);

            // Now set the rasterizer state.
            _deviceContext.Rasterizer.State = _rasterState;

            // Setup and create the viewport for rendering.
            _deviceContext.Rasterizer.SetViewport(0, 0, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height, 0, 1);
        }

        public void CreateMatrices()
        {
            // Setup and create the projection matrix.
            ProjectionMatrix = Matrix.PerspectiveFovLH(
                (float) (Math.PI/4),
                ((float) ConfigurationManager.Config.Width/ConfigurationManager.Config.Height),
                ConfigurationManager.Config.NearLimit, ConfigurationManager.Config.FarLimit);

            // Initialize the world matrix to the identity matrix.
            WorldMatrix = Matrix.Identity;

            // Create an orthographic projection matrix used for 2D rendering.
            OrthoMatrix = Matrix.OrthoLH(
                ConfigurationManager.Config.Width,
                ConfigurationManager.Config.Height,
                ConfigurationManager.Config.NearLimit,
                ConfigurationManager.Config.FarLimit);
        }

        public void Dispose()
        {
            if (_swapChain != null) // Before shutting down set swap chain to windowed mode 
                _swapChain.SetFullscreenState(false, null);
            DisposeAndSetToNull(_rasterState, _depthStencilView, _depthStencilState, _depthStencilBuffer, _renderTargetView, _device, _swapChain);
        }

        private void DisposeAndSetToNull(params IDisposable[] disposables)
        {
            for(int i = 0; i< disposables.GetLength(0); i++)
                if (disposables[i] != null)
                {
                    disposables[i].Dispose();
                    disposables[i] = null;
                }
        }

        public void BeginScene(float red, float green, float blue, float alpha)
        {
            // Clear the depth buffer.
            _deviceContext.ClearDepthStencilView(_depthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            // Clear the back buffer.
            _deviceContext.ClearRenderTargetView(_renderTargetView, new Color4(red, green, blue, alpha));
        }

        public void DrawScene() // Present the back buffer to the screen
        {
            _swapChain.Present(ConfigurationManager.Config.VSync ? 1 : 0, 0);
        }
    }
}
