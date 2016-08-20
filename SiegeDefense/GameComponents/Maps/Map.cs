using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Maps {
    public abstract class Map : _3DGameObject {
        public abstract float GetHeight(Vector3 position);
        public abstract Vector3 GetNormal(Vector3 position);
        public abstract bool Moveable(Vector3 position);
        public Map() {
            Tag = "Map";
        }
    }
}
