using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public abstract class GamePhysics : GameObjectComponent {
        public static Vector3 gravityForce { get; private set; } = new Vector3(0, -9.8f, 0);

        protected Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }
    }
}
