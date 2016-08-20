using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Cameras {
    public class Camera : _3DGameObject {
        public virtual Matrix ViewMatrix { get; set; }
        public virtual Matrix ProjectionMatrix { get; set; }

        public virtual Vector3 Target { get; set; } = Vector3.Forward;
        public override Vector3 Position { get; set; } = Vector3.Zero;
        public override Vector3 Up { get; set; } = Vector3.Up;

        public override Vector3 Forward {
            get {
                return Vector3.Normalize(Target - Position);
            }
        }
        public override Vector3 Left {
            get {
                return Vector3.Normalize(Vector3.Cross(Up, Forward));
            }
        }

        public Camera() {
            Tag = "Camera";
        }
    }
}
