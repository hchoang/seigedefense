using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class PathFinder {
        public static List<INode> AStar(INode start, INode end, IHeuristic heuristic) {
            Dictionary<INode, INode> comeFrom = new Dictionary<INode, INode>();
            Dictionary<INode, double> costSoFar = new Dictionary<INode, double>();

            PriorityQueue<INode> open = new PriorityQueue<INode>();
            open.Add(start, 0);

            comeFrom[start] = start;
            costSoFar[start] = 0;

            while(open.Count() > 0) {
                INode current = open.Pop();

                if (current.Equals(end)) {
                    break;
                }

                foreach(KeyValuePair<INode, double> entry in current.adjacentNodes) {
                    INode nextNode = entry.Key;
                    double nextCost = entry.Value;
                    double newCost = costSoFar[current] + nextCost;
                    if (!costSoFar.ContainsKey(nextNode) || newCost < costSoFar[nextNode]) {
                        costSoFar[nextNode] = newCost;
                        double priority = newCost + heuristic.GetHeuristic(nextNode, end);
                        open.Add(nextNode, priority);
                        comeFrom[nextNode] = current;
                    }
                }
            }

            if (!comeFrom.ContainsKey(end)) return null;

            List<INode> ret = new List<INode>();

            INode tracer = end;
            while (tracer != start) {
                ret.Insert(0, tracer);
                tracer = comeFrom[tracer];
            }

            return ret;
        } 
    }
}
