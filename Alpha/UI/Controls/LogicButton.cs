using Alpha.UI.Coordinates;

namespace Alpha.UI.Controls
{
    using System;
    class LogicButton : Control
    {
        public event CustomEventHandler Clicked;

        public LogicButton(IGame game, String id, UniRectangle coordinates)
            : base(game, id, coordinates)
        {}

        public override void Initialize()
        {}
        
        public override void OnMouseReleased()
        {
            Clicked.Raise();
        }

        public override string ComponentType
        {
            get { return "button"; }
        }

        protected override void DisposeItem()
        {}
    }
}