using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class ExplosiveTruckAI : AI {

        public float nearDistance { get; set; }
        public float fireRange { get; set; }

        public override void componentInit() {
            stateMachine = StateMachine.ReadFromXML(Game.Content.RootDirectory + @"\AI\ExplosiveTruck.xml");

            stateMap.Add("WANDER", new WanderingState() { AIObject = AIObject });
            stateMap.Add("CHASE", new ChaseState() { AIObject = AIObject });
            stateMap.Add("FIRE", new FireState() { AIObject = AIObject });

            conditionMap.Add("PLAYER_NEAR", IsPlayerNear);
            conditionMap.Add("PLAYER_FAR", IsPlayerFar);
            conditionMap.Add("PLAYER_IN_FIRE_RANGE", IsPlayerInFireRange);

            currentState = stateMachine.initState;

            nearDistance = float.Parse(stateMachine.configurationMap["NEAR_DISTANCE"]);
            fireRange = float.Parse(stateMachine.configurationMap["FIRE_RANGE"]);
        }

        public bool IsPlayerNear() {
            float playerDistance = (Player.transformation.Position - AIObject.transformation.Position).Length();

            return fireRange < playerDistance && playerDistance <= nearDistance;
        }

        public bool IsPlayerFar() {
            float playerDistance = (Player.transformation.Position - AIObject.transformation.Position).Length();
            return nearDistance < playerDistance;
        }

        public bool IsPlayerInFireRange() {
            float playerDistance = (Player.transformation.Position - AIObject.transformation.Position).Length();
            return playerDistance <= fireRange;
        }

        public override void Update(GameTime gameTime) {

            base.Update(gameTime);

            if (steeringForce == Vector3.Zero) {
                AILandVehicle.physics.ForwardForce = 0;
                return;
            }

            steeringForce = TankBehaviour.AdvoidObstacleBehaviour(AILandVehicle, steeringForce, Map);
            steeringForce = TankBehaviour.AdvoidObstacleBehaviour(AILandVehicle, steeringForce, Map, AILandVehicle.physics.MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            steeringForce = new Vector3(steeringForce.X, 0, steeringForce.Z);

            Vector3 flattenedForward = AILandVehicle.transformation.Forward;
            flattenedForward.Y = 0;
            flattenedForward.Normalize();

            Vector3 flattendedLeft = AILandVehicle.transformation.Left;
            flattendedLeft.Y = 0;
            flattendedLeft.Normalize();

            float steeringAngle = Utility.RotationAngleCalculator(flattenedForward, steeringForce, flattendedLeft);

            if (Math.Abs(steeringAngle) > 0.1f) {
                float rotateForce = steeringAngle > 0 ? 0.1f : -0.1f;
                AILandVehicle.physics.RotateForce = rotateForce;
            } else {
                AILandVehicle.physics.RotateForce = 0;
            }

            AILandVehicle.physics.ForwardForce = 1;
        }
    }
}
