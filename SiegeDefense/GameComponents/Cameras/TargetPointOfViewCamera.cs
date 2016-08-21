using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.Cameras {
    public class TargetPointOfViewCamera : Camera {
        private _3DGameObject targetToFollow;
        private Vector3 positionOffset;

        public TargetPointOfViewCamera(_3DGameObject targetToFollow, Vector3 positionOffset) {
            this.targetToFollow = targetToFollow;
            this.positionOffset = positionOffset;

            float aspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 2, 1000);
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix() {
            Vector3 offset = Vector3.Transform(positionOffset, targetToFollow.RotationMatrix);
            Position = targetToFollow.Position + offset;
            Target = targetToFollow.Position - targetToFollow.Forward * 100;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        public override void Update(GameTime gameTime) {
            UpdateViewMatrix();
        }
    }
}
