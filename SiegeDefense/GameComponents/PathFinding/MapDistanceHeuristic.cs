using SiegeDefense.GameComponents.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.PathFinding {
    public class MapDistanceHeuristic : IHeuristic {
        public double GetHeuristic(INode a, INode b) {
            MapNode node1 = (MapNode)a;
            MapNode node2 = (MapNode)b;
            return (node1.Position - node2.Position).Length();
        }
    }
}
