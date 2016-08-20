using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.Cameras {
    public class FollowTargetCamera : Camera {
        private _3DGameObject targetToFollow;
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

        public FollowTargetCamera(_3DGameObject target, float distance) {
            targetToFollow = target;
            targetDistance = distance;
            cameraToTargetDirection = targetToFollow.Forward;

            
            float aspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 1000);

            UpdateCameraViewMatrix();
        }

        private void UpdateCameraViewMatrix() {
            Target = targetToFollow.Position;
            cameraToTargetDirection = targetToFollow.Forward;
            Vector3 newPosition = targetToFollow.Position + cameraToTargetDirection * targetDistance;

            if (map.Moveable(newPosition)) {
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
