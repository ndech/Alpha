using System;
using System.Collections.Generic;
using SharpDX;

namespace Alpha
{
    class PathfindingNode : IComparable<PathfindingNode>
    {
        public Province Province { get; set; }
        public PathfindingNode Parent { get; set; }

        public double PathLength { get; set; }
        public double EstimateRemainingDistance { get; set; }
        public double Cost { get { return PathLength + EstimateRemainingDistance; } }

        public PathfindingNode(Province site, double estimateRemainingDistance, PathfindingNode parent = null)
        {
            Province = site;
            EstimateRemainingDistance = estimateRemainingDistance;
            PathLength = parent == null ? 0 : parent.PathLength + Vector3.Distance(parent.Province.Center, site.Center);
            Parent = parent;
        }

        public int CompareTo(PathfindingNode other)
        {
            return Comparer<double>.Default.Compare(Cost, other.Cost);
        }
    }
}
