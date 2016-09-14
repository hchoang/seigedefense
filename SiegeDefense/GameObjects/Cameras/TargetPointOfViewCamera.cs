using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class TargetPointOfViewCamera : Camera {
        private Transformation targetTransform;
        private Vector3 positionOffset;

        public TargetPointOfViewCamera(GameObject target, Vector3 positionOffset) {
            this.targetTransform = target.transformation;
            this.positionOffset = positionOffset;

            float aspectRatio = GraphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 2, 1500);
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix() {
            Vector3 offset = Vector3.Transform(positionOffset, targetTransform.RotationMatrix);
            Position = targetTransform.Position + offset;
            Target = targetTransform.Position + targetTransform.Forward * 100;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        public override void Update(GameTime gameTime) {
            UpdateViewMatrix();
        }
    }
}
