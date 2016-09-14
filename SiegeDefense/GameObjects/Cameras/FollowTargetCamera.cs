using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class FollowTargetCamera : Camera {
        private Transformation targetTransformation;
        private float targetDistance;
        private Vector3 cameraToTargetDirection;
        private Map _map;
        private Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public FollowTargetCamera(GameObject target, float distance) {
            targetTransformation = target.transformation;
            targetDistance = distance;
            cameraToTargetDirection = targetTransformation.Forward;

            
            float aspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1000);

            UpdateCameraViewMatrix();
        }

        private void UpdateCameraViewMatrix() {
            Target = targetTransformation.Position;
            cameraToTargetDirection = targetTransformation.Forward;
            Vector3 newPosition = targetTransformation.Position + cameraToTargetDirection * targetDistance;

            if (map.IsInsideMap(newPosition)) {
                float height = map.GetHeight(newPosition) + 20;
                Position = new Vector3(newPosition.X, height, newPosition.Z);
            } else {
                Position = new Vector3(newPosition.X, Position.Y, newPosition.Z);
            }

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        public override void Update(GameTime gameTime) {
            UpdateCameraViewMatrix();
        }
    }
}
