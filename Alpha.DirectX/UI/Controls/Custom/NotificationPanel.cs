using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class NotificationPanel : Panel
    {
        public NotificationPanel(IContext context) : base(context, "notification_panel", new UniRectangle(), Color.WhiteSmoke)
        { }
    }
}
