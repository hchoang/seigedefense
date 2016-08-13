using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Cameras {
    public class Camera : GameObject {
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public virtual Vector3 Position { get; set; }
        public virtual Vector3 Target { get; set; }

        public virtual Vector3 Up { get; set; }
        public virtual Vector3 Forward { get; set; }
        public virtual Vector3 Side { get; set; }
    }
}
