
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class TankController : InputListenerComponent {

        private Tank controlledTank {
            get {
                return (Tank)baseObject;
            }
        }

        public override void Update(GameTime gameTime) {
            if (inputManager.isTriggered(GameInput.Fire)) {
                controlledTank.Fire();
            }

            // move
            float forwardForce = 0;
            int rotateDirection = 1;
            if (inputManager.GetValue(GameInput.Up) != 0)
                forwardForce = 1;
            else if (inputManager.GetValue(GameInput.Down) != 0) {
                forwardForce = -1;
                rotateDirection = -1;
            }
            controlledTank.physics.ForwardForce = forwardForce;

            // rotate
            float rotateForce = 0;
            if (inputManager.GetValue(GameInput.Left) != 0) {
                rotateForce = 0.1f;
            }
            if (inputManager.GetValue(GameInput.Right) != 0) {
                rotateForce = -0.1f;
            }
            
            rotateForce *= rotateDirection;
            controlledTank.physics.RotateForce = rotateForce;

            // rotate turret & cannon
            float turretRotationAngle = inputManager.GetValue(GameInput.Horizontal) * (float)gameTime.ElapsedGameTime.TotalSeconds * controlledTank.TurretRotateSpeed;
            float canonRotationAngle = inputManager.GetValue(GameInput.Vertical) * (float)gameTime.ElapsedGameTime.TotalSeconds * controlledTank.CanonRotateSpeed;

            if (turretRotationAngle != 0) {
                controlledTank.renderer.RotateTurret(turretRotationAngle);
            }

            if (canonRotationAngle != 0) {
                controlledTank.renderer.RotateCanon(canonRotationAngle);
            }
        }
    }
}
