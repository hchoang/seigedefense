using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Input;
using SiegeDefense.GameComponents.Maps;
using System;

namespace SiegeDefense.GameComponents.Cameras {
    public class FPSCamera : Camera {
        private GraphicsDevice graphicsDevice;

        public MultiTexturedHeightMap map { protected get; set; }

        public override Vector3 Forward {
            get {
                return Vector3.Normalize(Target - Position);
            }
        }
        public override Vector3 Side {
            get {
                return Vector3.Normalize(Vector3.Cross(Forward, Up));
            }
        }

        private float aspectRatio = 0.75f;
        private float nearPlane = 2;
        private float farPlane = 50000;
        private float maxZoom = MathHelper.PiOver4;
        private float minZoom = 0.2f;
        private float currentZoom = MathHelper.PiOver4;
        private float zoomSpeed = 0.02f;
        private float rotationSpeed = 0.2f;
        private IInputManager inputManager;

        public FPSCamera(Vector3 Position, Vector3 Target, Vector3 Up) {
            this.Position = Position;
            this.Target = Target;
            this.Up = Up;
        }

        protected override void LoadContent() {
            inputManager = Game.Services.GetService<IInputManager>();
            graphicsDevice = Game.GraphicsDevice;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);

            aspectRatio = graphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);
        }

        public override void Update(GameTime gameTime) {
            float zoomValue = (float)(inputManager.GetValue(GameInput.Zoom) * zoomSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (zoomValue != 0) {
                float fovAterZoom = currentZoom + zoomValue;
                if (minZoom <= fovAterZoom && fovAterZoom <= maxZoom) {
                    currentZoom = fovAterZoom;
                    ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);
                }
            }

            if (inputManager.isPressing(GameInput.Up)) {
                Vector3 goUpVector = Forward;
                goUpVector.Normalize();
                Position += goUpVector;
                Target += goUpVector;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            } else if (inputManager.isPressing(GameInput.Down)) {
                Vector3 goDownVector = -Forward;
                goDownVector.Normalize();
                Position += goDownVector;
                Target += goDownVector;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            }

            if (inputManager.isPressing(GameInput.Left)) {
                Vector3 goLeftVector = -Side;
                goLeftVector.Y = 0;
                goLeftVector.Normalize();
                Position += goLeftVector;
                Target += goLeftVector;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            } else if (inputManager.isPressing(GameInput.Right)) {
                Vector3 goRightVector = Side;
                goRightVector.Y = 0;
                goRightVector.Normalize();
                Position += goRightVector;
                Target += goRightVector;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            }

            if (inputManager.GetValue(GameInput.Vertical) != 0){
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Side, (float)-gameTime.ElapsedGameTime.TotalSeconds * inputManager.GetValue(GameInput.Vertical) * rotationSpeed);

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

            float height = map.GetHeight(Position) + 20;
            if (height != Position.Y) {
                Vector3 dY = new Vector3(0, height - Position.Y, 0);
                Position += dY;
                Target += dY;
                ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
            }
        }
    }
}
