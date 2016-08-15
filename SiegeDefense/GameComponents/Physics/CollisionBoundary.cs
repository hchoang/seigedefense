using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.Physics {
    public abstract class CollisionBoundary : _3DGameObject {
        public abstract bool IsInside(Vector3 point);
        public abstract float GroundDistance(Map map);
    }
}
