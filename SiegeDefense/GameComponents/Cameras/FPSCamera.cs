using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Input;
using SiegeDefense.GameComponents.Maps;
using System;

namespace SiegeDefense.GameComponents.Cameras {
    public class FPSCamera : Camera {
        private GraphicsDevice graphicsDevice;

        private Map map;
        private float aspectRatio = 0.75f;
        private float nearPlane = 2;
        private float farPlane = 1000;
        private float maxZoom = MathHelper.PiOver4;
        private float minZoom = 0.2f;
        private float currentZoom = MathHelper.PiOver4;
        private float zoomSpeed = 0.02f;
        private float rotationSpeed = 0.2f;
        private IInputManager inputManager;
        private float jumpForce = 0;

        public FPSCamera(Vector3 Position, Vector3 Target, Vector3 Up) {
            this.Position = Position;
            this.Target = Target;
            this.Up = Up;

            inputManager = Game.Services.GetService<IInputManager>();
            graphicsDevice = Game.GraphicsDevice;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);

            aspectRatio = graphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);
        }

        public override void GetDependentComponents() {
            map = (Map)FindObjectsByTag("Map")[0];

            base.GetDependentComponents();
        }

        public override void Update(GameTime gameTime) {

            // zoom
            float zoomValue = (float)(inputManager.GetValue(GameInput.Zoom) * zoomSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (zoomValue != 0) {
                float fovAterZoom = currentZoom + zoomValue;
                if (minZoom <= fovAterZoom && fovAterZoom <= maxZoom) {
                    currentZoom = fovAterZoom;
                    ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);
                }
            }

            // move
            Vector3 moveDirection = Vector3.Zero;
            if (inputManager.isPressing(GameInput.Up)) {
                Vector3 goUpVector = Forward;
                goUpVector.Y = 0;
                goUpVector.Normalize();
                moveDirection += goUpVector;
            } else if (inputManager.isPressing(GameInput.Down)) {
                Vector3 goDownVector = -Forward;
                goDownVector.Y = 0;
                goDownVector.Normalize();
                moveDirection += goDownVector;
            }

            if (inputManager.isPressing(GameInput.Left)) {
                Vector3 goLeftVector = Left;
                goLeftVector.Y = 0;
                goLeftVector.Normalize();
                moveDirection += goLeftVector;
            } else if (inputManager.isPressing(GameInput.Right)) {
                Vector3 goRightVector = -Left;
                goRightVector.Y = 0;
                goRightVector.Normalize();
                moveDirection += goRightVector;
            }
            Move(moveDirection);

            // look around
            if (inputManager.GetValue(GameInput.Vertical) != 0){
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Left, (float)gameTime.ElapsedGameTime.TotalSeconds * inputManager.GetValue(GameInput.Vertical) * rotationSpeed);

                Vector3 newTarget = Vector3.Transform(Forward, rotationMatrix) + Position;
                Vector3 newForward = Vector3.Normalize(newTarget - Position);
                if (-0.75 < newForward.Y && newForward.Y < 0.75) {
                    Target = newTarget;
                    ViewMatrix = Matrix.CreateLookAt(Position, newTarget, Up);
                }
            }

            if (inputManager.GetValue(GameInput.Horizontal) != 0) {
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Up, (float)-gameTime.ElapsedGameTime.TotalSeconds * inputManager.GetValue(GameInput.Horizontal) * rotationSpeed);
                
                Target = Vector3.Transform(Forward, rotationMatrix) + Position;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            }

            // update height & jump
            float height = map.GetHeight(Position) + 20;
            if (height != Position.Y && jumpForce == 0) {
                Vector3 dY = new Vector3(0, height - Position.Y, 0);
                Position += dY;
                Target += dY;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            }

            if (inputManager.isTriggered(GameInput.Jump)) {
                if (jumpForce == 0) {
                    jumpForce = 1.2f;
                }
            }

            Move(new Vector3(0, jumpForce, 0));
            
            jumpForce -= 1.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Position.Y <= height)
                jumpForce = 0;
        }

        public void Move(Vector3 direction) {

            Vector3 newPosition = Position + direction;
            if (!map.Moveable(newPosition))
                return;

            Position = newPosition;
            Target += direction;
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }
    }
}
