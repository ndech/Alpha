using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Fleets;
using Alpha.Core.Provinces;

namespace Alpha.Core.Movement
{
    public static class PathFinder
    {
        public static List<Step> CalculatePath(Fleet fleet, Zone destination)
        {
            return CalculatePath((IMovable)fleet, destination);
        }

        private static List<Step> CalculatePath(IMovable movable, Zone destination)
        {
            List<Step> steps = new List<Step>();
            if (movable.Location == destination || !movable.CanCross(destination))
                return steps;
            //Calculate path using A* algorithm
            SortedSet<PathfindingNode> openList = new SortedSet<PathfindingNode>(
                Comparer<PathfindingNode>.Create((a, b) => a.CompareTo(b)));
            HashSet<Zone> closedList = new HashSet<Zone>();
            openList.Add(new PathfindingNode(movable.Location, destination.DistanceWith(movable.Location)));
            bool pathFound = destination == movable.Location;
            while (!pathFound)
            {
                if (openList.Count == 0)
                    break;
                PathfindingNode currentNode = openList.First();
                foreach (Zone neighbourg in currentNode.Zone.Adjacencies.Select(a => a.Neighbourg).Where(s => movable.CanCross(s)))
                {
                    if (closedList.Contains(neighbourg) || openList.Any(n => n.Zone == neighbourg))
                        continue;
                    openList.Add(new PathfindingNode(neighbourg, destination.DistanceWith(neighbourg), currentNode));
                    if (neighbourg == destination) // Path found !
                    {
                        pathFound = true;
                        steps.Add(new Step(currentNode.Zone, destination));
                        while (currentNode.Parent != null)
                        {
                            steps.Add(new Step(currentNode.Parent.Zone, currentNode.Zone));
                            currentNode = currentNode.Parent;
                        }
                        steps.Reverse();
                        break;
                    }
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode.Zone);
            }
            if (steps.Count == 0)
                return steps;
            return steps;
        }
    }
}
