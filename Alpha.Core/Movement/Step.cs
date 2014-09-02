using System;
using Alpha.Core.Provinces;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Movement
{
    public class Step
    {
        public Province Source { get; private set; }
        public Province Destination { get; private set; }
        public Double Distance { get; private set; }

        public Double Duration(float speed)
        {
            return Distance/speed;
        }

        public Step(Province source, Province destination)
        {
            Source = source;
            Destination = destination;
            Distance = Vector3D.Distance(source.Center, destination.Center);
        }
    }
}