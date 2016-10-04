using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class EnemyTankAI : AI {
        public float nearValue { get; set; }
        public float tooNearValue { get; set; }

        public override void componentInit() {
            stateMap.Add("WANDER", new WanderingState() { AIObject = AIObject });
            stateMap.Add("CHASE", new ChaseState() { AIObject = AIObject });
            stateMap.Add("AIM", new AimState() { AIObject = AIObject });
            stateMap.Add("FIRE", new FireState() { AIObject = AIObject });

            conditionMap.Add("PLAYER_NEAR", IsPlayerNear);
            conditionMap.Add("PLAYER_FAR", IsPlayerFar);
            conditionMap.Add("PLAYER_TOO_NEAR", IsPlayerTooNear);
            conditionMap.Add("PLAYER_IN_FIRE_RANGE", IsPlayerInFireRange);

            stateMachine = StateMachine.ReadFromXML(Game.Content.RootDirectory + @"\AI\EnemyTank.xml");
            currentState = stateMachine.initState;

            nearValue = float.Parse(stateMachine.configurationMap["NEAR_DISTANCE"]);
            tooNearValue = float.Parse(stateMachine.configurationMap["TOO_NEAR_DISTANCE"]);
        }

        public bool IsPlayerNear() {
            float distance = (Player.transformation.Position - baseObject.transformation.Position).Length();
            return tooNearValue < distance && distance <= nearValue;
        }

        public bool IsPlayerTooNear() {
            float distance = (Player.transformation.Position - baseObject.transformation.Position).Length();
            return distance <= tooNearValue;
        }

        public bool IsPlayerFar() {
            float distance = (Player.transformation.Position - baseObject.transformation.Position).Length();
            return distance > nearValue;
        }

        public bool IsPlayerInFireRange() {
            Matrix AITankCanonHeadMatrix = AITank.renderer.GetCanonHeadWorldMatrix();
            Vector3 target = Player.transformation.Position - AITank.transformation.Position;
            float angle = Utility.RotationAngleCalculator(AITankCanonHeadMatrix.Forward, target, AITankCanonHeadMatrix.Left);

            return (Math.Abs(angle) < 0.1f);
        }

        public override void Update(GameTime gameTime) {

            base.Update(gameTime);

            if (steeringForce == Vector3.Zero) {
                AITank.physics.ForwardForce = 0;
                return;
            }

            steeringForce = TankBehaviour.AdvoidObstacleBehaviour(AITank, steeringForce, Map);
            steeringForce = TankBehaviour.AdvoidObstacleBehaviour(AITank, steeringForce, Map, AITank.physics.MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            steeringForce = new Vector3(steeringForce.X, 0, steeringForce.Z);

            Vector3 flattenedForward = AITank.transformation.Forward;
            flattenedForward.Y = 0;
            flattenedForward.Normalize();

            Vector3 flattendedLeft = AITank.transformation.Left;
            flattendedLeft.Y = 0;
            flattendedLeft.Normalize();

            float steeringAngle = Utility.RotationAngleCalculator(flattenedForward, steeringForce, flattendedLeft);

            if (Math.Abs(steeringAngle) > 0.1f) {
                float rotateForce = steeringAngle > 0 ? 0.1f : -0.1f;
                AITank.physics.RotateForce = rotateForce;
            } else {
                AITank.physics.RotateForce = 0;
            }

            AITank.physics.ForwardForce = 1;
        }
    }
}
