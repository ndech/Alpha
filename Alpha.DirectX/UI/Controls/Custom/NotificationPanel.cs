using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class NotificationPanel : Panel
    {
        public NotificationPanel(IContext context, UniRectangle coordinates) : base(context, "notification_panel", coordinates, SharpDX.Color.WhiteSmoke)
        {

        }
    }
}
