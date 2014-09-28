using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class Minimap : Control
    {
        private TexturedRectangle _mapTexture;
        private Buffer _overlayVertexBuffer;
        public Minimap(IContext context, UniRectangle coordinates) : base(context, "minimap", coordinates)
        {
        }

        public override string ComponentType
        {
            get { return "minimap"; }
        }

        protected override void DisposeItem()
        {

        }

        public override void Initialize()
        {
            Texture2D minimapTexture = new Texture2D(Context.DirectX.Device, new Texture2DDescription
            {
                Width = Size.X,
                Height = Size.Y,
                Format = Format.R8G8B8A8_UNorm_SRgb,
                BindFlags = BindFlags.ShaderResource,
                ArraySize = 1,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1,0)
            });

            int rowPitch = 4 * minimapTexture.Description.Width;
            int byteCount = rowPitch*minimapTexture.Description.Height;
            var byteArray = new byte[byteCount];
            for (int i = 0; i < minimapTexture.Description.Width; i++)
            {
                for (int j = 0; j < minimapTexture.Description.Height; j++)
                {
                    if (Context.World.ProvinceManager.ClosestProvince(new Vector3D((
                        (float)i * Context.World.Size.X) / Size.X, 0, ((float)(Size.Y-j) * Context.World.Size.Y) / Size.Y)) is LandProvince)
                    {
                        byteArray[4 * (j * minimapTexture.Description.Width + i)] = 90;
                        byteArray[4 * (j * minimapTexture.Description.Width + i) + 1] = 184;
                        byteArray[4 * (j * minimapTexture.Description.Width + i) + 2] = 128;
                        byteArray[4 * (j * minimapTexture.Description.Width + i) + 3] = 255;
                    }
                    else
                    {
                        byteArray[4 * (j * minimapTexture.Description.Width + i)] = 80;
                        byteArray[4 * (j * minimapTexture.Description.Width + i) + 1] = 105;
                        byteArray[4 * (j * minimapTexture.Description.Width + i) + 2] = 199;
                        byteArray[4 * (j * minimapTexture.Description.Width + i) + 3] = 255;
                    }
                }
            }
            DataStream dataStream = new DataStream(byteCount, true, true);
            dataStream.Write(byteArray, 0, byteCount);
            DataBox data = new DataBox(dataStream.DataPointer, rowPitch, byteCount);
            Context.DirectX.DeviceContext.UpdateSubresource(data, minimapTexture);

            _mapTexture = new TexturedRectangle(Context, new ShaderResourceView(Context.DirectX.Device, minimapTexture), Size);
        }

        public override void OnControlDragged()
        {
            Vector2I mousePosition = Context.UiManager.MousePosition;
            
            if (InBounds(mousePosition))
            {
                Vector3 currentLookAt = new Picker(Context, Context.ScreenSize/2).GroundIntersection;
                Vector3 targetLookAt = new Vector3(((float)(mousePosition.X-Position.X)*Context.World.Size.X)/Size.X,0,
                    ((float)(Size.Y-(mousePosition.Y - Position.Y)) * Context.World.Size.Y) / Size.Y);
                Context.Camera.Position += targetLookAt - currentLookAt;
            }
        }

        public override void OnMouseClicked()
        {
            Context.UiManager.SetMousePointer(MousePointer.CursorType.Drag);
        }

        public override void OnMouseReleased()
        {
            Context.UiManager.SetMousePointer(MousePointer.CursorType.Default);
        }

        protected override void Update(double delta)
        {
            List<Vector2I> corners = new List<Vector2I>
            {
                new Vector2I(0, 0),
                new Vector2I(0, Context.ScreenSize.Y),
                new Vector2I(Context.ScreenSize.X, Context.ScreenSize.Y),
                new Vector2I(Context.ScreenSize.X, 0)
            };
            var list = corners.Select(c => new Picker(Context, c).GroundIntersection)
                .Select(vector => new Vector2I((int)(vector.X/Context.World.Size.X*Size.X), (int)(vector.Z/Context.World.Size.Y*Size.Y))).ToList();
            list.Add(list[0]);
            VertexDefinition.PositionColor[] vertices = list.Select(p => new VertexDefinition.PositionColor
            {
                color = Color.White.ToVector4(),
                position = new Vector3(p.X, Size.Y-p.Y, 0)
            }).ToArray();
            if (_overlayVertexBuffer != null)
            {
                UpdateOverlayVertexBuffer(Context.DirectX.DeviceContext, _overlayVertexBuffer, vertices, 5);
                return;
            }
            _overlayVertexBuffer = Buffer.Create(Context.DirectX.Device, vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionColor>() * 5,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
        }


        private void UpdateOverlayVertexBuffer(DeviceContext deviceContext, Resource overlayVertexBuffer, VertexDefinition.PositionColor[] vertices, int vertexCount)
        {
            var dataBox = deviceContext.MapSubresource(overlayVertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(dataBox.DataPointer, vertices, 0, vertexCount);
            deviceContext.UnmapSubresource(overlayVertexBuffer, 0);
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _mapTexture.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            deviceContext.InputAssembler.SetVertexBuffers(0, 
                new VertexBufferBinding(_overlayVertexBuffer, Utilities.SizeOf<VertexDefinition.PositionColor>(), 0));
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
            deviceContext.Rasterizer.SetScissorRectangle(Position.X, Position.Y, Position.X + Size.X, Position.Y + Size.Y);
            Context.Shaders.ColorShader.RenderNotIndexed(deviceContext, 5, worldMatrix, viewMatrix, projectionMatrix);
            deviceContext.Rasterizer.SetScissorRectangle(0, 0, Context.ScreenSize.X, Context.ScreenSize.Y);
        }
    }
}
