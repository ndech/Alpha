﻿using System;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI.Controls
{
    abstract class Control : UiComponent
    {
        public UniRectangle Coordinates { get; set; }
        public bool Hovered { get; protected set; }
        public bool Overlay { get; set; }
        public bool Visible { get; set; }
        protected Control(IContext context, string id, UniRectangle coordinates)
            : base(context, id)
        {
            Coordinates = coordinates;
            Overlay = false;
            Hovered = false;
            Visible = true;
        }

        public void LoadStyle()
        {
            
        }
        public abstract void Initialize();

        public override bool InBoundsAndActive(Vector2I position)
        {
            if (Overlay || !IsVisible()) return false;
            return    position.X >= Position.X
                   && position.X < Position.X + Size.X
                   && position.Y >= Position.Y
                   && position.Y < Position.Y + Size.Y;
        }

        public bool InBounds(Vector2I position)
        {
            return IsVisible()
                   && position.X >= Position.X
                   && position.X < Position.X + Size.X
                   && position.Y >= Position.Y
                   && position.Y < Position.Y + Size.Y;
        }
        
        public virtual void OnMouseLeft()
        { }
        
        public virtual void OnMouseEntered()
        { }

        public virtual void OnMouseClicked()
        { }

        public virtual void OnMouseReleasedInBounds()
        { }
        public virtual void OnMouseReleasedOutOfBounds()
        { }

        public override Vector2I Position
        {
            get { return Coordinates.Position.ToOffset(Parent.Size) + Parent.Position; }
        }

        public override Vector2I RelativePosition
        {
            get { return Coordinates.Position.ToOffset(Parent.Size); }
        }
        public override Vector2I Size
        {
            get { return Coordinates.Size.ToOffset(Parent.Size); }
        }

        public void MouseEntered()
        {
            Hovered = true;
            OnMouseEntered();
        }

        public void MouseLeft()
        {
            Hovered = false;
            OnMouseLeft();
        }
        
        public virtual void OnControlDragged(Vector2I move)
        {

        }

        public virtual void OnMouseReleased()
        {

        }

        public override bool IsVisible()
        {
            return Visible;
        }
    }
}
