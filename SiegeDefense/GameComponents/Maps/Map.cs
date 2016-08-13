using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Maps {
    public abstract class Map : _3DGameObject {
        public abstract bool isOverGround(Vector3 Position);
    }
}
