using Alpha.Core.Fleets;

namespace Alpha.Core.Notifications
{
    class MoveFleetNotification : Notification
    {
        public Fleet Fleet { get; private set; }
        internal MoveFleetNotification(Fleet fleet)
        {
            Fleet = fleet;
        }

        public override bool ValidForRealm(RealmToken realm)
        {
            return true;
        }
    }
}
