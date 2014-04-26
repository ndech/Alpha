using Alpha.Toolkit.Math;
using Alpha.UI.Controls;
using SharpDX;

namespace Alpha.UI.Screens
{
    abstract class Screen : UiComponent
    {
        protected readonly IUiManager UiManager;

        public override Vector2I Size
        {
            get { return UiManager.ScreenSize; }
        }

        public override Vector2I Position
        {
            get { return Vector2I.Zero; }
        }

        public override Vector2I RelativePosition
        {
            get { return Vector2I.Zero; }
        }

        protected Control ClickedControl { get; set; }

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

        private Control _hoveredControl = null;
        protected Control HoveredControl 
        { 
            get { return _hoveredControl; }
            set
            {
                //If the new hovered control is the same as previously, no need to do anything
                if (ReferenceEquals(_hoveredControl, value))
                    return;
                if (_hoveredControl != null)
                    _hoveredControl.OnMouseLeft();
                _hoveredControl = value;
                if (_hoveredControl != null)
                    _hoveredControl.OnMouseEntered();
            } 
        }

        protected Screen(IGame game) : base(game)
        {
            UiManager = game.Services.GetService<IUiManager>();
        }

        public void OnMouseMoved(Vector2I position)
        {
            //Todo : if activated control
            //If we are still hovering the same component, we search if we hover one of it's child
            UiComponent node;
            if (HoveredControl != null && HoveredControl.InBounds(position))
                node = HoveredControl;
            else // If not, we search the full tree
                node = this;
            HoveredControl = node.GetHoveredControl(position);
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
        }
    }
}
