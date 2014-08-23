using Alpha.Common;

namespace Alpha.Core.Notifications
{
    public abstract class Notification
    {
        public abstract bool ValidForRealm(RealmToken realm);
    }
}
