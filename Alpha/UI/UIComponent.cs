using System.Collections.Generic;
using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    abstract class UiComponent
    {
        protected readonly IGame Game;
        protected readonly List<Control> Controls;
        protected UiComponent Parent = null;

        protected UiComponent(IGame game)
        {
            Game = game;
            Controls = new List<Control>();
        }
        
        public void UpdateTree(double delta)
        {
            Update(delta);
            foreach (Control control in Controls)
                control.UpdateTree(delta);
        }

        public void RenderTree(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            Render(deviceContext, worldMatrix * DisplacementMatrix, viewMatrix, projectionMatrix);
            foreach (Control control in Controls)
                control.RenderTree(deviceContext, worldMatrix * DisplacementMatrix, viewMatrix, projectionMatrix);
        }

        protected virtual void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix) { }

        protected virtual void Update(double delta) { }

        public UiComponent Register(Control component)
        {
            Controls.Add(component);
            component.Parent = this;
            component.Initialize();
            return this;
        }

        public UiComponent Register(params UiComponent[] components)
        {
            foreach (UiComponent component in components)
                Register(component);
            return this;
        }

        public abstract bool InBounds(Vector2I position);

        internal Control GetHoveredControl(Vector2I position)
        {
            if (!InBounds(position))
                return null;
            foreach (Control control in Controls)
            {
                Control hoveredControl = control.GetHoveredControl(position);
                if (hoveredControl != null)
                    return control;
            }
            return this as Control;
        }

        public virtual Matrix DisplacementMatrix { get { return Matrix.Translation(RelativePosition.X, RelativePosition.Y, 0); } }

        public abstract Vector2I Size { get; }
        public abstract Vector2I Position { get; }
        public abstract Vector2I RelativePosition { get; }
    }
}
