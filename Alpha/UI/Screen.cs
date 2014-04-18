using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    abstract class Screen
    {
        protected List<UiComponent> _components;
        protected IGame _game;

        protected Screen(IGame game)
        {
            _components = new List<UiComponent>();
            _game = game;
        }

        public virtual void Update(double delta)
        {
            foreach (UiComponent component in _components)
                component.Update(delta);
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (UiComponent component in _components)
                component.Render(deviceContext, viewMatrix, projectionMatrix);
        }
    }
}
