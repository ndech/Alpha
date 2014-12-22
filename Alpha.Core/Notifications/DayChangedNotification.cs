namespace Alpha.Core.Notifications
{
    public class DayChangedNotification : Notification
    {
        public override bool ValidForRealm(RealmToken realm)
        {
            return true;
        }
    }
}
