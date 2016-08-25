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
        public float Mass { get; set; } = 1;
        public Vector3 Force { get; set; } = Vector3.Zero;
        public Vector3 Acceleration {
            get {
                return Force / Mass;
            }
        }
        public Vector3 Velocity { get; set; } = Vector3.Zero;

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
            Force += force;
        }

        public override void Update(GameTime gameTime) {

            // update target position
            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            baseModel.Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // update force
            if (map.IsInsideMap(baseModel.Position)) {
                float groundDistance = baseModel.Position.Y - map.GetHeight(baseModel.Position);
                if (groundDistance > 0) {
                    Force += gravityForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else {
                    Force = new Vector3(Force.X, 0, Force.Z);
                }
            }
        }
    }
}
