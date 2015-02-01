using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Alpha.DirectX.Shaders;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.DXGI;

namespace Alpha.DirectX.UI.Controls
{
    class PieChart : Control
    {
        private readonly int _sliceNumber;
        private readonly Buffer _vertexBuffer;
        private TextureShader _shader;
        private Func<List<Tuple<CustomColor, double, String, String>>> _valuesGenerator;
        private ShaderResourceView _colorTexture;
        private List<PieChartInfo> _data = new List<PieChartInfo>();

        class PieChartInfo
        {
            public PieChartInfo(int sliceCount, CustomColor color, string label, string tooltip)
            {
                SliceCount = sliceCount;
                Color = color;
                Label = label;
                Tooltip = tooltip;
            }

            public int SliceCount { get; private set; }
            public CustomColor Color { get; private set; }
            public String Label { get; private set; }
            public String Tooltip { get; private set; }

            public override string ToString()
            {
                return SliceCount + " " + Label;
            }
        }

        public PieChart(IContext context, string id, UniRectangle coordinates, int sliceNumber, Func<List<Tuple<CustomColor, double, string, string>>> valuesGenerator)
            : base(context, id, coordinates)
        {
            _sliceNumber = sliceNumber;
            _valuesGenerator = valuesGenerator;
            _shader = Context.Shaders.TextureShader;
            var vertices = new VertexDefinition.PositionTexture[_sliceNumber * 3];
            for (int i = 0; i < sliceNumber; i++)
            {
                Vector2 texture = new Vector2((0.5f + i)/_sliceNumber, 0.5f);
                vertices[i * 3] = new VertexDefinition.PositionTexture
                {
                    texture = texture,
                    position = new Vector3(0, 0, 0)
                };
                vertices[i * 3 + 2] = new VertexDefinition.PositionTexture
                {
                    texture = texture,
                    position = new Vector3((float)Math.Sin(i*(Math.PI * 2) / _sliceNumber), (float)Math.Cos(i*(Math.PI * 2) / _sliceNumber), 0)
                };
                vertices[i * 3 + 1] = new VertexDefinition.PositionTexture
                {
                    texture = texture,
                    position = new Vector3((float)Math.Sin((i+1) * (Math.PI * 2) / _sliceNumber), (float)Math.Cos((i+1) * (Math.PI * 2) / _sliceNumber), 0)
                };
            }
            _vertexBuffer = Buffer.Create(context.DirectX.Device, vertices,
                new BufferDescription
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<VertexDefinition.PositionTexture>() * _sliceNumber * 3,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                });
            _data = new List<PieChartInfo>
            {
                new PieChartInfo(_sliceNumber, new CustomColor(0.2f, 0.2f, 0.2f), "NA", "NA")
            };
            _colorTexture = GenerateTexture(Context, _sliceNumber);
        }


        public override string ComponentType
        {
            get { return "barchart"; }
        }

        protected override void DisposeItem()
        { }

        public override void Initialize()
        { }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            double size = Math.Min(Size.X, Size.Y)*0.45 - 10;
            int stride = Utilities.SizeOf<VertexDefinition.PositionColor>(); //Gets or sets the stride between vertex elements in the buffer (in bytes). 
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, stride, 0));
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _shader.RenderNotIndexed(deviceContext, _sliceNumber * 3, Matrix.Scaling((int)size) * Matrix.Translation(Size.X/2, Size.Y/2, 0) * worldMatrix, viewMatrix, projectionMatrix, _colorTexture);
        }

        public void Refresh()
        {
            _data.Clear();
            var data = _valuesGenerator();
            double total = data.Sum(x => x.Item2);
            double current = 0;
            foreach (Tuple<CustomColor, double, string, string> tuple in data)
            {
                current += tuple.Item2;
                _data.Add(new PieChartInfo(Convert.ToInt32(current*_sliceNumber/total) - _data.Sum(x=>x.SliceCount), tuple.Item1, tuple.Item3, tuple.Item4));
            }
            int y = Convert.ToInt32(current * _sliceNumber / total);
            int sum = _data.Sum(x => x.SliceCount);
            Debug.Assert(sum == _sliceNumber, "All pie chart slices should have data attached");
            UpdateTexture(Context, _colorTexture, _sliceNumber);
        }

        private ShaderResourceView GenerateTexture(IContext context, int sliceCount)
        {
            Texture1D provinceTexture = new Texture1D(context.DirectX.Device, new Texture1DDescription
            {
                Width = sliceCount,
                Format = Format.R32G32B32A32_Float,
                BindFlags = BindFlags.ShaderResource,
                ArraySize = 1,
                MipLevels = 1
            });

            int rowPitch = 16 * sliceCount;
            var byteArray = new byte[rowPitch];
            for (int i = 0; i < sliceCount; i++)
            {
                CustomColor color = ColorInSlice(i);
                Array.Copy(BitConverter.GetBytes(color.Red), 0, byteArray, i * 16, 4);
                Array.Copy(BitConverter.GetBytes(color.Green), 0, byteArray, i * 16 + 4, 4);
                Array.Copy(BitConverter.GetBytes(color.Blue), 0, byteArray, i * 16 + 8, 4);
                Array.Copy(BitConverter.GetBytes(1.0f), 0, byteArray, i * 16 + 12, 4);
            }
            DataStream dataStream = new DataStream(rowPitch, true, true);
            dataStream.Write(byteArray, 0, rowPitch);
            DataBox data = new DataBox(dataStream.DataPointer, rowPitch, rowPitch);
            context.DirectX.DeviceContext.UpdateSubresource(data, provinceTexture);

            return new ShaderResourceView(context.DirectX.Device, provinceTexture);
        }

        private void UpdateTexture(IContext context, ShaderResourceView texture, int sliceCount)
        {
            int rowPitch = 16 * sliceCount;
            var byteArray = new byte[rowPitch];
            for(int i=0; i<sliceCount; i++)
            {
                CustomColor color = ColorInSlice(i);
                Array.Copy(BitConverter.GetBytes(color.Red), 0, byteArray, i * 16, 4);
                Array.Copy(BitConverter.GetBytes(color.Green), 0, byteArray, i * 16 + 4, 4);
                Array.Copy(BitConverter.GetBytes(color.Blue), 0, byteArray, i * 16 + 8, 4);
                Array.Copy(BitConverter.GetBytes(1.0f), 0, byteArray, i * 16 + 12, 4);
            }
            DataStream dataStream = new DataStream(rowPitch, true, true);
            dataStream.Write(byteArray, 0, rowPitch);
            DataBox data = new DataBox(dataStream.DataPointer, rowPitch, rowPitch);
            context.DirectX.DeviceContext.UpdateSubresource(data, texture.Resource);
        }

        private CustomColor ColorInSlice(int i)
        {
            Debug.Assert(i >= 0);
            Debug.Assert(i < _sliceNumber);
            int cumulative = 0;
            foreach (PieChartInfo pieChartInfo in _data)
            {
                cumulative += pieChartInfo.SliceCount;
                if (cumulative > i)
                    return pieChartInfo.Color;
            }
            throw new Exception("Invalid index");
        }
    }
}
