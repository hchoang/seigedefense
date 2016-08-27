using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Models;
using System;

namespace SiegeDefense.GameComponents.Physics {
    public class GamePhysics : GameObject {
        public static Vector3 gravityForce { get; private set; } = new Vector3(0, -9.8f, 0);

        protected BaseModel baseModel { get; set; }
        public float Mass = 1;
        public Vector3 Acceleration { get; set; } = Vector3.Zero;
        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public float MaxSpeed { get; set; } = 5;

        protected Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public GamePhysics(BaseModel baseModel) {
            this.baseModel = baseModel;
        }

        public void AddForce(Vector3 Force) {
            Acceleration += Force / Mass;
        }

        public override void Update(GameTime gameTime) {

            // calculate target position, velocity & acceleration
            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Velocity.Length() > MaxSpeed) {
                Velocity = Velocity * MaxSpeed / Velocity.Length();
            }
            baseModel.Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // apply gravity
            if (map.IsInsideMap(baseModel.Position)) {
                float groundDistance = baseModel.Position.Y - map.GetHeight(baseModel.Position);
                if (groundDistance > 0) {
                    Acceleration += gravityForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else {
                    Acceleration = new Vector3(Acceleration.X, 0, Acceleration.Z);
                }
            }
        }
    }
}
