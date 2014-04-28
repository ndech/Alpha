using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Alpha.Graphics
{
    abstract class Rectangle
    {
        protected Buffer _vertexBuffer;
        protected Buffer _indexBuffer;
        protected int _indexCount;
        protected DeviceContext _deviceContext;

        private Vector2I _size;
        public Vector2I Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    Update();
                }
            }
        }

        public Rectangle()
        {
            
        }

        public abstract void Update();

        public abstract void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix,
            Matrix projectionMatrix);
    }
}
