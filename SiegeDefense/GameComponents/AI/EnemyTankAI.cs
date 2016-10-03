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
        public StateMachine stateMachine { get; set; }
        public Dictionary<string, AIState> stateMap { get; set; } = new Dictionary<string, AIState>();
        public Dictionary<string, Func<bool>> conditionMap { get; set; } = new Dictionary<string, Func<bool>>();
        public float nearValue { get; set; }
        public float tooNearValue { get; set; }
        public string currentState { get; set; }

        public void Init() {
            stateMachine = StateMachine.ReadFromXML(Game.Content.RootDirectory + @"\AI\EnemyTank.xml");

            stateMap.Add("WANDER", new WanderingState() { AITank = AITank });
            stateMap.Add("CHASE", new ChaseState() { AITank = AITank });
            stateMap.Add("AIM", new AimState() { AITank = AITank });
            stateMap.Add("FIRE", new FireState() { AITank = AITank });

            conditionMap.Add("PLAYER_NEAR", IsPlayerNear);
            conditionMap.Add("PLAYER_FAR", IsPlayerFar);
            conditionMap.Add("PLAYER_TOO_NEAR", IsPlayerTooNear);
            conditionMap.Add("PLAYER_IN_FIRE_RANGE", IsPlayerInFireRange);

            currentState = stateMachine.initState;

            nearValue = float.Parse(stateMachine.configurationMap["NEAR_DISTANCE"]);
            tooNearValue = float.Parse(stateMachine.configurationMap["TOO_NEAR_DISTANCE"]);
        }

        public bool IsPlayerNear() {
            float distance = (Player.transformation.Position - AITank.transformation.Position).Length();
            return tooNearValue < distance && distance <= nearValue;
        }

        public bool IsPlayerTooNear() {
            float distance = (Player.transformation.Position - AITank.transformation.Position).Length();
            return distance <= tooNearValue;
        }

        public bool IsPlayerFar() {
            float distance = (Player.transformation.Position - AITank.transformation.Position).Length();
            return distance > nearValue;
        }

        public bool IsPlayerInFireRange() {
            Matrix AITankCanonHeadMatrix = AITank.renderer.GetCanonHeadWorldMatrix();
            Vector3 target = Player.transformation.Position - AITank.transformation.Position;
            float angle = Utility.RotationAngleCalculator(AITankCanonHeadMatrix.Forward, target, AITankCanonHeadMatrix.Left);

            return (Math.Abs(angle) < 0.1f);
        }

        private bool isInitialized = false;
        public override void Update(GameTime gameTime) {
            if (!isInitialized) {
                Init();
                isInitialized = true;
            }

            foreach (KeyValuePair<string, string> transition in stateMachine.transitionMap[currentState]) {
                bool conditionMeet = conditionMap[transition.Key]();
                if (conditionMeet) {
                    stateMap[currentState].OnExit();
                    currentState = transition.Value;
                    stateMap[currentState].OnEnter();
                    break;
                }
            }

            foreach (KeyValuePair<string, string> subState in stateMachine.subStateMap[currentState]) {
                bool conditionMeet = conditionMap[subState.Key]();
                if (conditionMeet) {
                    stateMap[subState.Value].Update(gameTime);
                } else {
                    stateMap[subState.Value].PassiveUpdate(gameTime);
                }
            }

            foreach (string stateName in stateMap.Keys) {
                if (stateName.Equals(currentState)) {
                    stateMap[stateName].Update(gameTime);
                } else {
                    stateMap[stateName].PassiveUpdate(gameTime);
                }
            }

            if (steeringForce == Vector3.Zero) {
                AITank.physics.ForwardForce = 0;
                return;
            }

            if (reverse) {
                steeringForce = -steeringForce;
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
                if (reverse) {
                    rotateForce = -rotateForce;
                }
                AITank.physics.RotateForce = rotateForce;
            } else {
                AITank.physics.RotateForce = 0;
                AITank.physics.RotateAcceleration = 0;
            }

            AITank.physics.ForwardForce = reverse ? -1 : 1;
        }
    }
}
