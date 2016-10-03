using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {

    public abstract class AIState : GameObjectComponent {
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

        private static Tank _player;
        protected static Tank player {
            get {
                if (_player == null) {
                    _player = (Tank)FindObjectsByTag("Player")[0];
                }
                return _player;
            }
        }

        public Tank AITank { get; set; }

        private AI _AI;
        protected AI AI {
            get {
                if (_AI == null) {
                    _AI = AITank.GetComponent<AI>();
                }
                return _AI;
            }
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void PassiveUpdate(GameTime gameTime) { }
    }
}
