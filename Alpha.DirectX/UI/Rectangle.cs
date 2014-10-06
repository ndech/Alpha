using Alpha.Toolkit.Math;
using SharpDX.Direct3D11;

namespace Alpha.DirectX.UI
{
    abstract class Rectangle
    {
        protected Buffer VertexBuffer;
        protected Buffer IndexBuffer;
        protected int IndexCount;
        protected DeviceContext DeviceContext;

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

        public abstract void Update();
    }
}
