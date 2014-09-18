using Alpha.Core.Fleets;

namespace Alpha.Core.Notifications
{
    public class FleetMovedNotification : Notification
    {
        public Fleet Fleet { get; private set; }
        internal FleetMovedNotification(Fleet fleet)
        {
            Fleet = fleet;
        }

        public override bool ValidForRealm(RealmToken realm)
        {
            return true;
        }
    }
}
