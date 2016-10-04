using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public abstract class State : GameObjectComponent {
        protected static Random RNG = new Random();

        private Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        private static OnlandVehicle _player;
        protected static OnlandVehicle player {
            get {
                if (_player == null) {
                    _player = (OnlandVehicle)FindObjectsByTag("Player")[0];
                }
                return _player;
            }
        }

        public _3DGameObject AIObject { get; set; }
        public OnlandVehicle AIVehicle {
            get {
                return (OnlandVehicle)AIObject;
            }
        }

        private AI _AI;
        protected AI AI {
            get {
                if (_AI == null) {
                    _AI = AIObject.GetComponent<AI>();
                }
                return _AI;
            }
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void PassiveUpdate(GameTime gameTime) { }
    }
}
