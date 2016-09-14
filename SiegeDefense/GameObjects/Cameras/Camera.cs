using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public abstract class Camera : GameObject {
        public virtual Matrix ViewMatrix { get; set; }
        public virtual Matrix ProjectionMatrix { get; set; }

        public virtual Vector3 Target { get; set; } = Vector3.Forward;
        public virtual Vector3 Position { get; set; } = Vector3.Zero;
        public virtual Vector3 Up { get; set; } = Vector3.Up;

        public virtual Vector3 Forward {
            get {
                return Vector3.Normalize(Target - Position);
            }
        }
        public virtual Vector3 Left {
            get {
                return Vector3.Normalize(Vector3.Cross(Up, Forward));
            }
        }

        public Camera() {
            Tag = "Camera";
        }
    }
}
