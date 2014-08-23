using Alpha.Core.Fleets;

namespace Alpha.Core.Notifications
{
    public class NewFleetNotification : Notification
    {
        public Fleet Fleet { get; private set; }
        internal NewFleetNotification(Fleet fleet)
        {
            Fleet = fleet;
        }

        public override bool ValidForRealm(RealmToken realm)
        {
            return true;
        }
    }
}
