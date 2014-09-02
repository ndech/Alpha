using System.Collections.Generic;
using Alpha.Core.Fleets;
using Alpha.Core.Notifications;

namespace Alpha.DirectX.UI
{
    class NotificationResolver
    {
        internal event CustomEventHandler<Fleet> NewFleetMoveOrder;
        internal event CustomEventHandler<Fleet> FleetMoved; 
        internal void Process(IEnumerable<Notification> notifications)
        {
            foreach (Notification notification in notifications)
            {
                if(notification is MoveFleetNotification)
                    NewFleetMoveOrder.Raise(((MoveFleetNotification)notification).Fleet);
                if(notification is MoveFleetNotification)
                    FleetMoved.Raise(((MoveFleetNotification)notification).Fleet);
            }
        }
    }
}
