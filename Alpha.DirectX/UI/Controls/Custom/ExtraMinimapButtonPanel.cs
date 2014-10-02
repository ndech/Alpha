using System;
using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ExtraMinimapButtonPanel : Panel
    {
        private readonly Func<bool> _toggled;

        public override bool IsVisible()
        {
            return _toggled();
        }

        public ExtraMinimapButtonPanel(IContext context, Func<bool> toggled) : base(context, "extra_minimap_buttons_panel", new UniRectangle(), SharpDX.Color.DarkGray)
        {
            _toggled = toggled;
        }
    }
}
