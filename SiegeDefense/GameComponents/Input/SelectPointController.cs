using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class SelectPointController : InputListenerComponent {

        public string state { get; set; } = "Selecting";
        public bool selectable { get; set; } = true;

        private Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }
        private Camera _camera;
        protected Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }

        public _3DGameObject StartPoint {
            get {
                return (_3DGameObject)baseObject;
            }
        }

        public override void Update(GameTime gameTime) {
            if (state == "Selecting") {
                Vector3 interactPosition = Vector3.Zero;
                interactPosition.X = inputManager.GetValue(GameInput.PointerX);
                interactPosition.Y = inputManager.GetValue(GameInput.PointerY);

                Viewport vp = Game.GraphicsDevice.Viewport;

                interactPosition.Z = 0;
                Vector3 nearPlanePoint = vp.Unproject(interactPosition, camera.ProjectionMatrix, camera.ViewMatrix, transformation.WorldMatrix);
                interactPosition.Z = 1;
                Vector3 farPlanePoint = vp.Unproject(interactPosition, camera.ProjectionMatrix, camera.ViewMatrix, transformation.WorldMatrix);

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
                        intersectPoint.Y = map.GetHeight(intersectPoint);
                        baseObject.transformation.Position = intersectPoint;

                        if (!map.IsAccessibleByFoot(intersectPoint)) {
                            baseObject.GetComponent<BillboardRenderer>()[0].maskColor = Color.Black;
                            selectable = false;
                        } else {
                            baseObject.GetComponent<BillboardRenderer>()[0].maskColor = Color.White;
                            selectable = true;
                        }
                    } else {
                        baseObject.GetComponent<BillboardRenderer>()[0].maskColor = Color.Black;
                        selectable = false;
                    }
                } else {
                    baseObject.GetComponent<BillboardRenderer>()[0].maskColor = Color.Red;
                    selectable = false;
                }
            }

            if (inputManager.isTriggered(GameInput.Fire) && selectable) {
                state = "Selected";
            }

            if (inputManager.isTriggered(GameInput.Fire2)) {
                Vector3 interactPosition = Vector3.Zero;
                interactPosition.X = inputManager.GetValue(GameInput.PointerX);
                interactPosition.Y = inputManager.GetValue(GameInput.PointerY);

                Viewport vp = Game.GraphicsDevice.Viewport;

                interactPosition.Z = 0;
                Vector3 nearPlanePoint = vp.Unproject(interactPosition, camera.ProjectionMatrix, camera.ViewMatrix, transformation.WorldMatrix);
                interactPosition.Z = 1;
                Vector3 farPlanePoint = vp.Unproject(interactPosition, camera.ProjectionMatrix, camera.ViewMatrix, transformation.WorldMatrix);

                Vector3[] boundingCorners = StartPoint.collider.baseBoundingBox.GetCorners();
                Matrix worldMatrix = baseObject.transformation.WorldMatrix;
                Vector3.Transform(boundingCorners, ref worldMatrix, boundingCorners);

                Ray ray1 = new Ray(nearPlanePoint, farPlanePoint - nearPlanePoint);
                float? result1 = ray1.Intersects(BoundingBox.CreateFromPoints(boundingCorners));

                if (result1 != null) {
                    Game.Components.Remove(baseObject);
                }
            }

        }

        public Vector3 GetIntersectPoint(Vector3 point1, Vector3 point2) {
            if ((point1 - point2).Length() < 1) {
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
    }
}
