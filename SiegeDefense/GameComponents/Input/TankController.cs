
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class TankController : GameObjectComponent {
        private IInputManager inputManager;

        private Tank controlledTank {
            get {
                return (Tank)baseObject;
            }
        }

        public TankController() {
            inputManager = Game.Services.GetService<IInputManager>();
        }

        public override void Update(GameTime gameTime) {
            if (inputManager.isTriggered(GameInput.Fire))
            {
                controlledTank.Fire();
            }

            // move & rotate
            Vector3 moveDirection = Vector3.Zero;
            int rotationDirection = 1;
            if (inputManager.GetValue(GameInput.Up) != 0)
                moveDirection += controlledTank.transformation.Forward;
            else if (inputManager.GetValue(GameInput.Down) != 0) {
                moveDirection -= controlledTank.transformation.Forward;
                rotationDirection = -1;
            }

            if (moveDirection != Vector3.Zero) {
                moveDirection = Vector3.Normalize(moveDirection) * controlledTank.MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                controlledTank.Move(moveDirection);
            }

            float rotationAngle = 0;
           // if (moveDirection != Vector3.Zero) {
                if (inputManager.GetValue(GameInput.Left) != 0) {
                    rotationAngle += controlledTank.RotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (inputManager.GetValue(GameInput.Right) != 0) {
                    rotationAngle -= controlledTank.RotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
          //  }

            if (rotationAngle != 0) {
                rotationAngle *= rotationDirection;
                controlledTank.RotateTank(rotationAngle);
            }
            // end move & rotate

            //rotate wheels
            float travelDistance = moveDirection.Length();
            
            if (travelDistance > 0)
            {
                controlledTank.renderer.RotateWheels(-rotationDirection);
            }

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
