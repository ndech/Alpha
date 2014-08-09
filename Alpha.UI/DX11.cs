using System;
using System.Linq;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace Alpha.UI
{
    class Dx11 : IDisposable
    {
        private bool _disposed;
        private Rational _refreshRate;
        private RenderTargetView _renderTargetView;
        private Texture2D _depthStencilBuffer;
        private DepthStencilState _depthStencilState;
        private DepthStencilState _depthDisabledStencilState;
        private RasterizerState _rasterStateSolid;
        private RasterizerState _rasterStateWireFrame;
        private BlendState _alphaEnabledBlendState;
        private BlendState _alphaDisabledBlendState;
        private int _maxQualityLevel;

        private bool _isZBufferEnabled;
        private bool _isBlendingEnabled;
        private bool _isWireframeEnabled;
        public DepthStencilView DepthStencilView { get; set; }
        public DepthStencilView RenderToTextureDepthStencilView { get; set; }
        public Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }
        public SwapChain SwapChain { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        public Matrix OrthoMatrix { get; private set; }
        public int VideoCardMemorySize { get; private set; }
        public String VideoCardName { get; private set; }


        public Dx11(RenderForm form)
        {
            AcquireGpu();
            CreateDeviceAndSwapChain(form);
            InitializeBuffers();
            CreateMatrices();
        }
        public void AcquireGpu()
        {
            Factory factory = new Factory();
            Adapter gpuAdapter = factory.Adapters.First();
            Output monitor = gpuAdapter.Outputs.First();

            _refreshRate = GetHighestAvailableRefreshFrequency(monitor);

            VideoCardMemorySize = gpuAdapter.Description.DedicatedVideoMemory >> 10 >> 10; //Convert bits to MB
            VideoCardName = gpuAdapter.Description.Description;

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
            FeatureLevel[] featureLevels = { FeatureLevel.Level_11_0, FeatureLevel.Level_10_0 };
            Device = new Device(DriverType.Hardware, DeviceCreationFlags.None, featureLevels);

            if(ConfigurationManager.Config.AntiAliasing)
                _maxQualityLevel = Device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, 4);

            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1, // Set to a single back buffer (double buffering)
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, _refreshRate, Format.R8G8B8A8_UNorm),
                IsWindowed = ConfigurationManager.Config.WindowedMode,
                OutputHandle = form.Handle,
                SampleDescription = ConfigurationManager.Config.AntiAliasing ? new SampleDescription(4, _maxQualityLevel-1) : new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            SwapChain = new SwapChain(new Factory(), Device, swapChainDescription);

            DeviceContext = Device.ImmediateContext;

            SwapChain.GetParent<Factory>().MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);
        }

        public void InitializeBuffers()
        {
            //Create back buffer
            Texture2D backBuffer = Resource.FromSwapChain<Texture2D>(SwapChain, 0);
            _renderTargetView = new RenderTargetView(Device, backBuffer);
            backBuffer.Dispose();

            //Create the depth/stencil buffer
			var depthBufferDesc = new Texture2DDescription
			{
				Width = ConfigurationManager.Config.Width,
				Height = ConfigurationManager.Config.Height,
				MipLevels = 1,
				ArraySize = 1,
				Format = Format.D24_UNorm_S8_UInt,
				SampleDescription = ConfigurationManager.Config.AntiAliasing ? new SampleDescription(4, _maxQualityLevel-1) : new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				BindFlags = BindFlags.DepthStencil,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None
			};
			_depthStencilBuffer = new Texture2D(Device, depthBufferDesc);

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
            _depthStencilState = new DepthStencilState(Device, depthStencilDesc);

            DepthStencilStateDescription depthDisabledStencilDesc = new DepthStencilStateDescription
            {
                IsDepthEnabled = false,
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

            _depthDisabledStencilState = new DepthStencilState(Device, depthDisabledStencilDesc);

            // Set the depth stencil state.
            _isZBufferEnabled = true;
            DeviceContext.OutputMerger.SetDepthStencilState(_depthStencilState, 1);

            // Initialize and set up the depth stencil view.
            DepthStencilViewDescription depthStencilViewDesc;
            if(ConfigurationManager.Config.AntiAliasing)
                depthStencilViewDesc = new DepthStencilViewDescription
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2DMultisampled,
                    Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource()
                };
            else
                depthStencilViewDesc = new DepthStencilViewDescription
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 }
                };

            // Create the depth stencil view.
            DepthStencilView = new DepthStencilView(Device, _depthStencilBuffer, depthStencilViewDesc);
            
            RenderToTextureDepthStencilView = new DepthStencilView(Device, new Texture2D(Device, new Texture2DDescription
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
			}), new DepthStencilViewDescription
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 }
            });


            // Bind the render target view and depth stencil buffer to the output render pipeline.
            DeviceContext.OutputMerger.SetTargets(DepthStencilView, _renderTargetView);

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
            _rasterStateSolid = new RasterizerState(Device, rasterDesc);

            rasterDesc.FillMode = FillMode.Wireframe;
            
            _rasterStateWireFrame = new RasterizerState(Device, rasterDesc);

            // Now set the rasterizer state.
            DeviceContext.Rasterizer.State = _rasterStateSolid;

            // Setup and create the viewport for rendering.
            DeviceContext.Rasterizer.SetViewport(0, 0, ConfigurationManager.Config.Width, ConfigurationManager.Config.Height);

            var blendStateDescription = new BlendStateDescription();
            blendStateDescription.RenderTarget[0].IsBlendEnabled = true;
            blendStateDescription.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDescription.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDescription.RenderTarget[0].BlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            blendStateDescription.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            blendStateDescription.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            // Create the blend state using the description.
            _alphaEnabledBlendState = new BlendState(Device, blendStateDescription);

            blendStateDescription.RenderTarget[0].IsBlendEnabled = false;
            // Create the blend state using the description.
            _alphaDisabledBlendState = new BlendState(Device, blendStateDescription);
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
            if (_disposed) return;
            if (SwapChain != null && SwapChain.IsFullScreen) // Before shutting down set swap chain to windowed mode 
                SwapChain.SetFullscreenState(false, null);
            DisposeHelper.DisposeAndSetToNull(_rasterStateSolid, DepthStencilView, _depthStencilState, _depthDisabledStencilState, _depthStencilBuffer, _renderTargetView, Device, SwapChain);
            _disposed = true;
        }

        public void BeginScene(float red, float green, float blue, float alpha)
        {
            // Clear the depth buffer.
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            // Clear the back buffer.
            DeviceContext.ClearRenderTargetView(_renderTargetView, new Color4(red, green, blue, alpha));
        }

        public void DrawScene() // Present the back buffer to the screen
        {
            SwapChain.Present(ConfigurationManager.Config.VSync ? 1 : 0, 0);
        }

        public void EnableZBuffer()
        {
            //if (_isZBufferEnabled) return;
            DeviceContext.OutputMerger.SetDepthStencilState(_depthStencilState, 1);
            _isZBufferEnabled = true;
        }

        public void DisableZBuffer()
        {
            if (!_isZBufferEnabled) return;
            DeviceContext.OutputMerger.SetDepthStencilState(_depthDisabledStencilState, 1);
            _isZBufferEnabled = false;
        }

        public void EnableAlphaBlending()
        {
            if (_isBlendingEnabled) return;
            DeviceContext.OutputMerger.SetBlendState(_alphaEnabledBlendState, new Color4(0.0f));
            _isBlendingEnabled = true;
        }

        public void DisableAlphaBlending()
        {
            if (!_isBlendingEnabled) return;
            DeviceContext.OutputMerger.SetBlendState(_alphaDisabledBlendState, new Color4(0.0f));
            _isBlendingEnabled = false;
        }

        public void EnableWireFrame()
        {
            if (_isWireframeEnabled) return;
            DeviceContext.Rasterizer.State = _rasterStateWireFrame;
            _isWireframeEnabled = true;
        }

        public void DisableWireFrame()
        {
            if (!_isWireframeEnabled) return;
            DeviceContext.Rasterizer.State = _rasterStateSolid;
            _isWireframeEnabled = false;
        }

        public void SetBackBufferAsRenderTarget()
        {
            DeviceContext.OutputMerger.SetRenderTargets(DepthStencilView, _renderTargetView);
        }
    }
}
