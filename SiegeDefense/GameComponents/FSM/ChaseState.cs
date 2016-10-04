using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class ChaseState : State {

        public float UpdateChasePathTime { get; set; } = 1;
        public float UpdateChasePathCounter { get; set; } = 0;
        public List<MapNode> ChasePath { get; set; } = new List<MapNode>();

        private void UpdatChasePath() {
            MapNode startNode = map.GetNode(AIObject.transformation.Position);
            MapNode endNode = map.GetNode(player.transformation.Position);

            if (startNode == null || endNode == null) {
                return;
            }

            var tryPath = PathFinder.AStar(startNode, endNode, new MapDistanceHeuristic());

            if (tryPath != null) {
                ChasePath = tryPath.Cast<MapNode>().ToList();
            }
        }

        public override void Update(GameTime gameTime) {

            if (UpdateChasePathCounter >= UpdateChasePathTime) {
                UpdateChasePathCounter = 0;
                UpdatChasePath();
            }

            UpdateChasePathCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ChasePath.Count == 0) {
                return;
            }

            Vector3 steeringForce = ChasePath[0].Position - AIObject.transformation.Position;
            steeringForce.Normalize();

            AI.steeringForce = steeringForce;

            float nextPathDistance = (AIObject.transformation.Position - ChasePath[0].Position).Length();
            if (nextPathDistance < 10) {
                ChasePath.RemoveAt(0);
            }

            base.Update(gameTime);
        }
    }
}
