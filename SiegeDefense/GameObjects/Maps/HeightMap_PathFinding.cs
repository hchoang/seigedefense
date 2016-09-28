using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SiegeDefense {
    public class MapNode : Transformation, INode {
        private Dictionary<INode, double> _adjacentNodes = new Dictionary<INode, double>();
        public Dictionary<INode, double> adjacentNodes {
            get {
                return _adjacentNodes;
            }

            set {
                _adjacentNodes = value;
            }
        }

        public bool Moveable { get; set; }
    }

    public partial class HeightMap {
        private MapNode[] nodes;
        private void GenerateMapNode() {

            nodes = new MapNode[mapInfoWidth * mapInfoHeight];
            for (int i = 0; i < mapInfoWidth; i++) {
                for (int j = 0; j < mapInfoHeight; j++) {
                    Vector3 position = renderer.vertices[i + j * mapInfoWidth].Position;
                    nodes[i + j * mapInfoWidth] = new MapNode();
                    nodes[i + j * mapInfoWidth].Position = position;
                    if (!IsAccessibleByFoot(position)) {
                        nodes[i + j * mapInfoWidth].Moveable = false;
                    }
                }
            }

            for (int i = 0; i < mapInfoWidth; i++) {
                for (int j = 0; j < mapInfoHeight; j++) {
                    MapNode left = i > 0 ? nodes[(i - 1) + j * mapInfoWidth] : null;
                    MapNode right = i < mapInfoWidth - 1 ? nodes[(i + 1) + j * mapInfoWidth] : null;
                    MapNode top = j > 0 ? nodes[i + (j - 1) * mapInfoWidth] : null;
                    MapNode bottom = j < mapInfoHeight - 1 ? nodes[i + (j + 1) * mapInfoWidth] : null;

                    MapNode topLeft = top != null && left != null ? nodes[(i - 1) + (j - 1) * mapInfoWidth] : null;
                    MapNode bottomLeft = bottom != null && left != null ? nodes[(i - 1) + (j + 1) * mapInfoWidth] : null;
                    MapNode topRight = top != null && right != null ? nodes[(i + 1) + (j - 1) * mapInfoWidth] : null;
                    MapNode bottomRight = bottom != null && right != null ? nodes[(i + 1) + (j + 1) * mapInfoWidth] : null;

                    MapNode current = nodes[i + j * mapInfoWidth];

                    if (left != null && left.Moveable) current.adjacentNodes.Add(left, (left.Position - current.Position).Length());
                    if (right != null && right.Moveable) current.adjacentNodes.Add(right, (right.Position - current.Position).Length());
                    if (top != null && top.Moveable) current.adjacentNodes.Add(top, (top.Position - current.Position).Length());
                    if (bottom != null && bottom.Moveable) current.adjacentNodes.Add(bottom, (bottom.Position - current.Position).Length());

                    if (topLeft != null && topLeft.Moveable && top.Moveable && left.Moveable) current.adjacentNodes.Add(topLeft, (topLeft.Position - current.Position).Length());
                    if (bottomLeft != null && bottomLeft.Moveable && bottom.Moveable && left.Moveable) current.adjacentNodes.Add(bottomLeft, (bottomLeft.Position - current.Position).Length());
                    if (topRight != null && topRight.Moveable && top.Moveable && right.Moveable) current.adjacentNodes.Add(topRight, (topRight.Position - current.Position).Length());
                    if (bottomRight != null && bottomRight.Moveable && bottom.Moveable && right.Moveable) current.adjacentNodes.Add(bottomRight, (bottomRight.Position - current.Position).Length());
                }
            }

        }

        public MapNode getNode(Vector3 position) {
            Vector3 firstVertexPosition = renderer.vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int X = (int)(relativePosition.X / mapCellSize);
            int Y = (int)(relativePosition.Z / mapCellSize);
            int nextX = X + 1;
            int nextY = Y + 1;

            if (nextX == mapInfoWidth)
                nextX -= 2;
            if (nextY == mapInfoHeight)
                nextY -= 2;

            MapNode node = nodes[X + Y * mapInfoWidth];
            if (node.Moveable) return node;

            node = nodes[nextX + Y * mapInfoWidth];
            if (node.Moveable) return node;

            node = nodes[nextX + nextY * mapInfoWidth];
            if (node.Moveable) return node;

            node = nodes[X + nextY * mapInfoWidth];
            if (node.Moveable) return node;

            return null;
        }
    }
}
