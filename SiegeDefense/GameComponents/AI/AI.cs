using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public abstract class AI : GameObjectComponent {
        private GameObject _player;
        public GameObject Player {
            get {
                if (_player == null) {
                    _player = FindObjectsByTag("Player")[0];
                }
                return _player;
            }
            set {
                _player = value;
            }
        }

        private Map _map;
        protected Map Map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
            set {
                _map = value;
            }
        }

        public Vector3 steeringForce { get; set; } = Vector3.Zero;

        public StateMachine stateMachine { get; set; }
        public Dictionary<string, State> stateMap { get; set; } = new Dictionary<string, State>();
        public Dictionary<string, Func<bool>> conditionMap { get; set; } = new Dictionary<string, Func<bool>>();
        public string currentState { get; set; }

        // casting objects
        public _3DGameObject AIObject { get { return (_3DGameObject)baseObject; } }
        public OnlandVehicle AILandVehicle { get { return (OnlandVehicle)baseObject; } }
        public Tank AITank { get { return (Tank)baseObject; } }

        public override void Update(GameTime gameTime) {

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

            base.Update(gameTime);
        }
    }
}
