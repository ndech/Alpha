using System;
using Alpha.Core.Provinces;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Movement
{
    public class Step
    {
        public Zone Source { get; private set; }
        public Zone Destination { get; private set; }
        public Double Distance { get; private set; }

        public Double Duration(float speed)
        {
            return Distance/speed;
        }

        public Step(Zone source, Zone destination)
        {
            Source = source;
            Destination = destination;
            Distance = Vector3D.Distance(source.Center, destination.Center);
        }
    }
}