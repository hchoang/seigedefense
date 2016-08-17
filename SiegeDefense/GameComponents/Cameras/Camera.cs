using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Cameras {
    public class Camera : _3DGameObject {
        public virtual Matrix ViewMatrix { get; set; }
        public virtual Matrix ProjectionMatrix { get; set; }

        public virtual Vector3 Target { get; set; }
        public override Vector3 Position { get; set; }
        public override Vector3 Up { get; set; }

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
