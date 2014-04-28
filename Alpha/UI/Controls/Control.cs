using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;

namespace Alpha.UI.Controls
{
    abstract class Control : UiComponent
    {
        public UniRectangle Coordinates { get; set; }

        public override bool Visible { get; set; }
        public bool Overlay { get; set; }
        protected Control(IGame game, UniRectangle coordinates)
            : base(game)
        {
            Coordinates = coordinates;
            Overlay = false;
            Visible = true;
        }

        public abstract void Initialize();

        public override bool InBounds(Vector2I position)
        {
            if (Overlay) return false;
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

        public sealed override Vector2I Position
        {
            get { return Coordinates.Position.ToOffset(Parent.Size) + Parent.Position; }
        }

        public sealed override Vector2I RelativePosition
        {
            get { return Coordinates.Position.ToOffset(Parent.Size); }
        }
        public sealed override Vector2I Size
        {
            get { return Coordinates.Size.ToOffset(Parent.Size); }
        }
    }
}
