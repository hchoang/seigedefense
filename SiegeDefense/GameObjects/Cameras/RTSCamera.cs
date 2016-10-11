using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense{
    public class RTSCamera : Camera {
        private float moveSpeed = 500;
        private IInputManager inputManager;
        private float maxZoom = 1.0f;
        private float minZoom = 0.2f;
        private float currentZoom = MathHelper.PiOver4;
        private float zoomSpeed = 0.002f;
        private float aspectRatio;
        private float nearPlane = 2;
        private float farPlane = 500;

        private Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public RTSCamera(Vector3 target, float height, float angleDegree) {
            Target = target;

            Vector3 position = Vector3.Zero;
            position.X = target.X;
            position.Y = target.Y + height;
            float angleRadian = angleDegree * MathHelper.Pi / 180;
            position.Z = target.Z + height / (float)Math.Tan(angleRadian);
            Position = position;

            Up = Vector3.Cross(position - target, Vector3.Left);

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);

            aspectRatio = Game.GraphicsDevice.DisplayMode.AspectRatio;
            
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearPlane, farPlane);

            inputManager = Game.Services.GetService<IInputManager>();
        }

        public override void Update(GameTime gameTime) {

            Vector3 moveDirection = Vector3.Zero;

            if (inputManager.isPressing(GameInput.Up)) {
                moveDirection.Z += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            } else if (inputManager.isPressing(GameInput.Down)) {
                moveDirection.Z -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (inputManager.isPressing(GameInput.Left)) {
                moveDirection.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else if (inputManager.isPressing(GameInput.Right)) {
                moveDirection.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            Move(moveDirection);

            float zoomValue = inputManager.GetValue(GameInput.Zoom) * zoomSpeed;
            if (zoomValue != 0) {
                float fovAterZoom = currentZoom + zoomValue;
                if (minZoom <= fovAterZoom && fovAterZoom <= maxZoom) {
                    currentZoom = fovAterZoom;
                    ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);
                }
            }
        }

        public void Move(Vector3 direction) {
            Position += direction;
            Target += direction;
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }
    }
}
