﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI
{
    abstract class UiComponent : IDisposable
    {
        protected readonly IGame Game;
        protected readonly IUiManager UiManager;
        protected readonly List<Control> Controls;
        public UiComponent Parent { get; set; }
        public abstract bool Visible { get; set; }
        public abstract String ComponentType { get; }

        public String Id { get; private set; }
        
        protected UiComponent(IGame game, String id)
        {
            Game = game;
            Id = id;
            Parent = null;
            Controls = new List<Control>();
            UiManager = game.Services.Get<IUiManager>();
        }
        
        public void UpdateTree(double delta)
        {
            if (!Visible) return;
            Update(delta);
            foreach (Control control in Controls)
                control.UpdateTree(delta);
        }

        public void RenderTree(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (!Visible) return;
            Render(deviceContext, worldMatrix * DisplacementMatrix, viewMatrix, projectionMatrix);
            foreach (Control control in Controls)
                control.RenderTree(deviceContext, worldMatrix * DisplacementMatrix, viewMatrix, projectionMatrix);
        }

        public void ActivateTree()
        {
            Activate();
            foreach (Control control in Controls)
                control.ActivateTree();
        }

        public void DesactivateTree()
        {
            Desactivate();
            foreach (Control control in Controls)
                control.DesactivateTree();
        }

        protected virtual void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix) { }

        protected virtual void Update(double delta) { }

        protected virtual void Activate() { }

        protected virtual void Desactivate() { }
        public T Register<T>(T component) where T : Control
        {
            Controls.Add(component);
            component.Parent = this;
            component.LoadStyle();
            component.Initialize();
            return component;
        }

        public abstract bool InBounds(Vector2I position);

        internal Control GetHoveredControl(Vector2I position)
        {
            if (!InBounds(position))
                return null;
            for(int i = Controls.Count -1; i>=0; i--)
            {
                Control control = Controls[i];
                Control hoveredControl = control.GetHoveredControl(position);
                if (hoveredControl != null)
                    return control;
            }
            return this as Control;
        }

        public bool KeyPressed(Key key, char? character, bool repeat)
        {
            if (OnKeyPressed(key, character, repeat))
                return true;
            if (!Visible) return false;
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Control control = Controls[i];
                if (control.KeyPressed(key, character, repeat))
                    return true;
            }
            return false;
        }

        protected virtual bool OnKeyPressed(Key key, char? character, bool repeat)
        {
            return false;
        }

        public bool KeyReleased(Key key)
        {
            if (OnKeyReleased(key))
                return true;
            foreach (Control control in Controls)
                if (control.KeyReleased(key))
                    return true;
            return false;
        }

        public bool MouseScrolled(int delta)
        {
            if (OnMouseScrolled(delta))
                return true;
            foreach (Control control in Controls)
                if (control.MouseScrolled(delta))
                    return true;
            return false;
        }

        protected virtual bool OnMouseScrolled(int delta)
        {
            return false;
        }

        protected virtual bool OnKeyReleased(Key key)
        {
            return false;
        }

        public virtual Matrix DisplacementMatrix { get { return Matrix.Translation(RelativePosition.X, RelativePosition.Y, 0); } }

        public abstract Vector2I Size { get; }
        public abstract Vector2I Position { get; }
        public abstract Vector2I RelativePosition { get; }
        protected abstract void DisposeItem();
        public void Dispose()
        {
            foreach (Control control in Controls)
                control.Dispose();
            DisposeItem();
        }
    }
}
