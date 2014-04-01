using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    abstract class Screen
    {
        protected List<Component> _components;

        protected Screen()
        {
            _components = new List<Component>();
        }
        public abstract void Update(double delta);
        public abstract void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix);
    }
}
