using System.Collections.Generic;
using Alpha.Core.Fleets;
using Alpha.Core.Notifications;
using Alpha.Core.Provinces;

namespace Alpha.DirectX.UI
{
    class NotificationResolver
    {
        internal event CustomEventHandler<Fleet> NewFleetMoveOrder;
        internal event CustomEventHandler<Fleet> FleetMoved;
        internal event CustomEventHandler<Settlement> NewSettlement;
        internal void Process(IEnumerable<Notification> notifications)
        {
            foreach (Notification notification in notifications)
            {
                if(notification is NewFleetMoveOrderNotification)
                    NewFleetMoveOrder.Raise(((NewFleetMoveOrderNotification)notification).Fleet);
                if(notification is FleetMovedNotification)
                    FleetMoved.Raise(((FleetMovedNotification)notification).Fleet);
                if(notification is NewSettlementNotification)
                    NewSettlement.Raise(((NewSettlementNotification)notification).Settlement);
            }
        }
    }
}
