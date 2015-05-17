using System;
using System.Collections.Generic;
using Alpha.Toolkit.Math;

namespace Alpha.Core.Provinces
{
    class PathfindingNode : IComparable<PathfindingNode>
    {
        public Zone Zone { get; }
        public PathfindingNode Parent { get; private set; }
        public double PathLength { get; set; }
        public double EstimateRemainingDistance { get; set; }
        public double Cost { get { return PathLength + EstimateRemainingDistance; } }

        public PathfindingNode(Zone zone, double estimateRemainingDistance, PathfindingNode parent = null)
        {
            Zone = zone;
            EstimateRemainingDistance = estimateRemainingDistance;
            PathLength = parent == null ? 0 : parent.PathLength + Vector3D.Distance(parent.Zone.Center, zone.Center);
            Parent = parent;
        }

        public int CompareTo(PathfindingNode other)
        {
            return Comparer<double>.Default.Compare(Cost, other.Cost);
        }
    }
}
