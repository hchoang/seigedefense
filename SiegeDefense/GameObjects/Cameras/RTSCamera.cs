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

        public _3DGameObject playerFlag { get; set; }

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

            playerFlag = new _3DGameObject();
            playerFlag.renderer = new BillboardRenderer(Game.Content.Load<Texture2D>(@"Sprites\PlayerStartPoint"));
            playerFlag.AddComponent(playerFlag.renderer);
            playerFlag.transformation.ScaleMatrix = Matrix.CreateScale(5);
            Game.Components.Add(playerFlag);
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

            //if (inputManager.isTriggered(GameInput.Fire)) {
                Vector3 interactPosition = Vector3.Zero;
                interactPosition.X = inputManager.GetValue(GameInput.PointerX);
                interactPosition.Y = inputManager.GetValue(GameInput.PointerY);

                Viewport vp = Game.GraphicsDevice.Viewport;
                interactPosition.Z = 0;
                Vector3 nearPlanePoint = vp.Unproject(interactPosition, ProjectionMatrix, ViewMatrix, transformation.WorldMatrix);
                interactPosition.Z = 1;
                Vector3 farPlanePoint = vp.Unproject(interactPosition, ProjectionMatrix, ViewMatrix, transformation.WorldMatrix);

                Ray ray1 = new Ray(nearPlanePoint, farPlanePoint - nearPlanePoint);
                float? result1 = ray1.Intersects(map.GetBoundingBox());

                Ray ray2 = new Ray(farPlanePoint, nearPlanePoint - farPlanePoint);
                float? result2 = ray2.Intersects(map.GetBoundingBox());

                if (result1 != null && result2 != null) {
                    Vector3 intersectPoint1 = ray1.Position + (float)result1 * ray1.Direction;
                    Vector3 intersectPoint2 = ray2.Position + (float)result2 * ray2.Direction;

                    float point1Height = intersectPoint1.Y - map.GetHeight(intersectPoint1);
                    float point2Height = intersectPoint2.Y - map.GetHeight(intersectPoint2);

                    if (point1Height * point2Height < 0) {
                        Vector3 intersectPoint = GetIntersectPoint(intersectPoint1, intersectPoint2);
                    playerFlag.transformation.Position = intersectPoint;
                    }
                }
            //}
        }

        public Vector3 GetIntersectPoint(Vector3 point1, Vector3 point2) {
            if ( (point1 - point2).Length() < 1) {
                Vector3 result = point1;
                result.Y = map.GetHeight(result);
                return result;
            }

            Vector3 middlePoint = (point1 + point2) / 2;
            float midPointHeight = middlePoint.Y - map.GetHeight(middlePoint);
            if (midPointHeight < 0) {
                return GetIntersectPoint(point1, middlePoint);
            } else {
                return GetIntersectPoint(middlePoint, point2);
            }
        }

        public void Move(Vector3 direction) {
            Position += direction;
            Target += direction;
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }
    }
}
