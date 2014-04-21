using System;
using System.Collections.Generic;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    enum VerticalAlignment
    {
        Top,
        Bottom,
        Middle
    }

    enum HorizontalAlignment
    {
        Left,
        Right,
        Center
    }

    abstract class UiComponent
    {
        protected readonly IGame Game;
        protected readonly List<UiComponent> _components;
        protected UiComponent _parent;

        protected UiComponent(IGame game, Vector2I size, Vector2I position)
        {
            Game = game;
            Size = size;
            Position = position;
            _components = new List<UiComponent>();
        }
        
        public Vector2I Size { get; set; }
        public Vector2I Position { get; set; }

        public Int32 Height
        {
            get { return Size.Y; }
            set { Size = new Vector2I(value, Size.Y); }
        }

        public Int32 Width
        {
            get { return Size.X; }
            set { Size = new Vector2I(Size.X, value); }
        }
        
        public void Update(double delta)
        {
            UpdateComponent(delta);
            foreach (UiComponent component in _components)
                component.Update(delta);
        }

        public void Render(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix)
        {
            RenderComponent(deviceContext, viewMatrix, projectionMatrix);
            foreach (UiComponent component in _components)
                component.Render(deviceContext, viewMatrix, projectionMatrix);
        }

        protected virtual void RenderComponent(DeviceContext deviceContext, Matrix viewMatrix, Matrix projectionMatrix) { }

        protected virtual void UpdateComponent(double delta) { }

        public UiComponent Register(UiComponent component)
        {
            _components.Add(component);
            component._parent = this;
            return this;
        }

        public UiComponent Register(params UiComponent[] components)
        {
            foreach (UiComponent component in components)
                Register(component);
            return this;
        }
    }
}
