using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    abstract class Screen
    {
        protected List<Component> _components;
        protected IGame _game;

        protected Screen(IGame game)
        {
            _components = new List<Component>();
            _game = game;
        }
        public abstract void Update(double delta);

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (Component component in _components)
                component.Render(deviceContext, viewMatrix, projectionMatrix);
        }
    }
}
