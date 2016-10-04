using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class WanderingState : State {
        public float changeDestinationTime { get; set; } = 2;
        public float changeDestinationCounter { get; set; } = 2;

        public List<MapNode> pathList { get; set; } = new List<MapNode>();

        private void RandomDestination() {
            while (true) { 
                BoundingBox mapBounding = map.GetBoundingBox();
                Vector3 mapSize = mapBounding.Max - mapBounding.Min;
                Vector3 randomPosition = new Vector3((float)RNG.NextDouble(), 0, (float)RNG.NextDouble());
                
                randomPosition.X *= mapSize.X;
                randomPosition.Z *= mapSize.Z;
                randomPosition.Y = map.GetHeight(randomPosition);
                
                if (map.IsAccessibleByFoot(randomPosition)) {

                    MapNode startNode = map.GetNode(AIObject.transformation.Position);
                    MapNode endNode = map.GetNode(randomPosition);

                    if (startNode == null || endNode == null) {
                        continue;
                    }

                    var tryPath = PathFinder.AStar(startNode, endNode, new MapDistanceHeuristic());

                    if (tryPath != null) {
                        pathList = tryPath.Cast<MapNode>().ToList();
                        break;
                    }
                }
            }
        }

        public override void OnEnter() {
            changeDestinationCounter = changeDestinationTime;
        }

        public override void Update(GameTime gameTime) {

            if (changeDestinationCounter >= changeDestinationTime) {
                changeDestinationCounter = 0;
                RandomDestination();
            }

            changeDestinationCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (pathList.Count == 0) {
                return;
            }

            Vector3 steeringForce = pathList[0].Position - AIObject.transformation.Position;
            steeringForce.Normalize();

            AI.steeringForce = steeringForce;

            float nextPathDistance = (AIObject.transformation.Position - pathList[0].Position).Length();
            if (nextPathDistance < 10) {
                pathList.RemoveAt(0);
            }

            base.Update(gameTime);
        }
    }
}
