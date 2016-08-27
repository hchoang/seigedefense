using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Models;
using System;

namespace SiegeDefense.GameComponents.Physics {
    public class GamePhysics : GameObject {
        public static Vector3 gravityForce { get; private set; } = new Vector3(0, -9.8f, 0);

        protected BaseModel _baseModel;
        protected BaseModel baseModel {
            get {
                if (_baseModel == null) {
                    _baseModel = FindComponent<BaseModel>();
                }
                return _baseModel;
            }
        }
        public float Mass = 1;
        public Vector3 Acceleration = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;

        protected Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public void AddForce(Vector3 force) {
            Acceleration += force / Mass;
        }

        public override void Update(GameTime gameTime) {

            // calculate target position, velocity & acceleration
            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            baseModel.Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // apply gravity
            if (map.IsInsideMap(baseModel.Position)) {
                float groundDistance = baseModel.Position.Y - map.GetHeight(baseModel.Position);
                if (groundDistance > 0) {
                    Acceleration += gravityForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else {
                    Acceleration.Y = 0;
                }
            }
        }
    }
}
