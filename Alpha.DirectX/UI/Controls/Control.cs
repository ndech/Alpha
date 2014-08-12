using System;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;

namespace Alpha.DirectX.UI.Controls
{
    abstract class Control : UiComponent
    {
        public UniRectangle Coordinates { get; set; }
        public bool Hovered { get; protected set; }
        public override sealed bool Visible { get; set; }
        public bool Overlay { get; set; }
        protected Control(IContext context, String id, UniRectangle coordinates)
            : base(context, id)
        {
            Coordinates = coordinates;
            Overlay = false;
            Visible = true;
            Hovered = false;
        }

        public void LoadStyle()
        {
            
        }
        public abstract void Initialize();

        public override bool InBounds(Vector2I position)
        {
            if (Overlay || !Visible) return false;
            return    position.X >= Position.X
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

        public virtual void OnMouseReleased()
        { }
        public virtual void OnMouseClickCanceled()
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
        
        public virtual void OnControlDragged()
        {

        }
    }
}
