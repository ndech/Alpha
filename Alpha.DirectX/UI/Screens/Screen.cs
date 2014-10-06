using Alpha.DirectX.UI.Controls;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Screens
{
    abstract class Screen : UiComponent
    {
        public override string ComponentType
        {
            get { return "screen"; }
        }

        public override Vector2I Size
        {
            get { return Context.ScreenSize; }
        }

        public override Vector2I Position
        {
            get { return Vector2I.Zero; }
        }

        public bool Transparent { get; set; }

        public override Vector2I RelativePosition
        {
            get { return Vector2I.Zero; }
        }

        private Control _clickedControl;
        protected Control ClickedControl
        {
            get { return _clickedControl; }
            set
            {
                _clickedControl = value;
                if (_clickedControl != null)
                    _clickedControl.OnMouseClicked();
            }
        }

        private Control _activatedControl = null;
        public Control ActivatedControl
        {
            get { return _activatedControl; }
            set
            {
                if (ReferenceEquals(_activatedControl, value))
                    return;
                else
                {

                }
            } 
        }

        private Control _hoveredControl;
        protected Control HoveredControl 
        { 
            get { return _hoveredControl; }
            set
            {
                //If the new hovered control is the same as previously, no need to do anything
                if (ReferenceEquals(_hoveredControl, value))
                    return;
                if (_hoveredControl != null)
                    _hoveredControl.MouseLeft();
                _hoveredControl = value;
                if (_hoveredControl != null)
                    _hoveredControl.MouseEntered();
            } 
        }

        protected Screen(IContext context, string id, bool transparent = false) : base(context, id)
        {
            Transparent = transparent;
        }

        public void MouseMoved(Vector2I position)
        {
            if (ClickedControl != null)
            {
                ClickedControl.OnControlDragged();
                return;
            }
            //Todo : if activated control
            //If we are still hovering the same component, we search if we hover one of it's child
            UiComponent node;
            if (HoveredControl != null && HoveredControl.InBounds(position))
                node = HoveredControl;
            else // If not, we search the full tree
                node = this;
            HoveredControl = node.GetHoveredControl(position);
        }

        public void RecalculateActiveComponents()
        {
            ClickedControl = null;
            HoveredControl = null;
        }

        public override bool InBounds(Vector2I position)
        {
            return true;
        }

        public override Matrix DisplacementMatrix
        {
            get { return Matrix.Translation(-Size.X >> 1, -Size.Y >> 1, 0); }
        }

        public void Unregister()
        {
            HoveredControl = null;
            ClickedControl = null;
            ActivatedControl = null;
            DesactivateTree();
        }
        
        public void MouseClicked(Vector2I position, int button)
        {
            ClickedControl = HoveredControl;
            if(ClickedControl == null)
                OnMouseClicked(position, button);
        }

        protected virtual void OnMouseClicked(Vector2I position, int button)
        { }


        public void MouseReleased(Vector2I position, int button)
        {
            if(ClickedControl == null) return;
            if (ClickedControl.InBounds(position))
            {
                ClickedControl.OnMouseReleasedInBounds();
                ClickedControl.OnMouseReleased();
            }
            else
            {
                ClickedControl.OnMouseReleasedOutOfBounds();
                ClickedControl.OnMouseReleased();
                
            }
            ClickedControl = null;
        }


        public virtual bool MouseScrolled(int delta)
        {
            UiComponent currentComponent = HoveredControl;
            while (currentComponent != null && currentComponent != this )
            {
                if (currentComponent.OnMouseScrolled(delta))
                    return true;
                currentComponent = currentComponent.Parent;
            }
            return OnMouseScrolled(delta);
        }

        protected override void DisposeItem()
        { }
    }
}
