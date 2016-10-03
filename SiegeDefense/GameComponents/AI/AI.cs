using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public abstract class AI : GameObjectComponent {
        private GameObject _player;
        protected GameObject Player {
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
        public bool reverse { get; set; } = false;
    }
}
