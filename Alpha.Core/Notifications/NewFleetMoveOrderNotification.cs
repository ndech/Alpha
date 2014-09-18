using Alpha.Core.Fleets;

namespace Alpha.Core.Notifications
{
    public class NewFleetMoveOrderNotification : Notification
    {
        public Fleet Fleet { get; private set; }
        internal NewFleetMoveOrderNotification(Fleet fleet)
        {
            Fleet = fleet;
        }

        public override bool ValidForRealm(RealmToken realm)
        {
            return true;
        }
    }
}
