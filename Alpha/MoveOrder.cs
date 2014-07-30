using System;
using SharpDX;

namespace Alpha
{
    abstract class MoveOrder
    {
        public static Vector4 MainColor(DiplomaticStatus status)
        {
            if (status == DiplomaticStatus.AtWar)
                return (Vector4)Color.Red;
            if (status == DiplomaticStatus.Self)
                return (Vector4)Color.Green;
            return (Vector4)Color.White;
        }
        public static Vector4 BackgroundColor(DiplomaticStatus status)
        {
            if (status == DiplomaticStatus.AtWar)
                return (Vector4)Color.LightSalmon;
            if (status == DiplomaticStatus.Self)
                return (Vector4)Color.LimeGreen;
            return (Vector4)Color.LightGray;
        }
        public class Step
        {
            public Province Destination { get; private set; }
            public Int32 Duration { get; private set; }

            public Step(Province destination, Int32 duration)
            {
                Destination = destination;
                Duration = duration;
            }
        }
    }
}
