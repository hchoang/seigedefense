namespace SiegeDefense {
    public class MapDistanceHeuristic : IHeuristic {
        public double GetHeuristic(INode a, INode b) {
            MapNode node1 = (MapNode)a;
            MapNode node2 = (MapNode)b;
            return (node1.Position - node2.Position).Length();
        }
    }
}
