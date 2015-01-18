namespace Alpha.Core.Notifications
{
    public class DayUpdateDoneNotification : Notification
    {
        public override bool ValidForRealm(RealmToken realm)
        {
            return true;
        }
    }
}
