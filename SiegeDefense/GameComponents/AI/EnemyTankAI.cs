using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class EnemyTankAI : AI {

        public float SightRadius { get; set; } = 100f;

        private List<MapNode> paths = new List<MapNode>();

        private HeightMap heightMap {
            get { return (HeightMap)Map; }
        }

        private Tank AITank {
            get { return (Tank)baseObject; }
        }

        public override void Update(GameTime gameTime) {
            MapNode currentNode = heightMap.getNode(AITank.transformation.Position);
            if (paths.Count == 0) {
                paths = PathFinder.AStar(currentNode, heightMap.getNode(Player.transformation.Position), new MapDistanceHeuristic()).Cast<MapNode>().ToList();
            } else {
                // move
                Vector3 steeringForce = paths[0].Position - AITank.transformation.Position;
                steeringForce.Normalize();

                if (steeringForce != Vector3.Zero) {
                    AITank.physics.ForwardForce = 1;
                }


                // rotate
                AITank.physics.RotateForce = Utility.RotationAngleCalculator(AITank.transformation.Forward, steeringForce, AITank.transformation.Left);
                // project steering force to plane (Forward, Left)
                Vector3 normalizedUp = Vector3.Normalize(AITank.transformation.Up);
                float t = Vector3.Dot(normalizedUp, steeringForce);
                steeringForce = steeringForce - t * normalizedUp;
                float steeringAngle = Utility.RotationAngleCalculator(AITank.transformation.Forward, steeringForce, AITank.transformation.Left);
                AITank.physics.RotateForce = steeringAngle;

                if (currentNode == paths[0]) {
                    paths.RemoveAt(0);
                }
            }
        }

    }
}
