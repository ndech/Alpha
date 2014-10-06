using Alpha.Core.Provinces;

namespace Alpha.Core.Notifications
{
    public class NewSettlementNotification : Notification
    {
        public readonly Settlement Settlement;

        internal NewSettlementNotification(Settlement settlement)
        {
            Settlement = settlement;
        }

        public override bool ValidForRealm(RealmToken realm)
        {
            return Settlement.Province.Owner == realm.Realm;
        }
    }
}
