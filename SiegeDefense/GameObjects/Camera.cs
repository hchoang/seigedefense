using Microsoft.Xna.Framework;
using SiegeDefense.Input;

namespace SiegeDefense.GameObjects {
    public class Camera : GameObject {
        private GraphicsDeviceManager graphics;

        private float aspectRatio = 0.75f;
        private float nearPlane = 1;
        private float farPlane = 500;
        private float maxZoom = 2.0f;
        private float minZoom = 0.2f;
        private float currentZoom = MathHelper.PiOver4;
        private float zoomSpeed = 0.002f;
        private IInputManager inputManager;

        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public Camera(Vector3 position, Vector3 lookAt, Vector3 upDirection) {
            ViewMatrix = Matrix.CreateLookAt(position, lookAt, upDirection);

            graphics = Game.Services.GetService<GraphicsDeviceManager>();
            aspectRatio = graphics.GraphicsDevice.DisplayMode.AspectRatio;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);

            inputManager = Game.Services.GetService<IInputManager>();
        }

        public override void Update(GameTime gameTime) {
            float zoomValue = inputManager.GetValue(GameInput.Zoom) * zoomSpeed;

            if (zoomValue != 0) {
                float fovAterZoom = currentZoom + zoomValue;
                if (minZoom <= fovAterZoom && fovAterZoom <= maxZoom) {
                    currentZoom = fovAterZoom;
                    ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(currentZoom, aspectRatio, nearPlane, farPlane);
                }
            }
        }
    }
}
