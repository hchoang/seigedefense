using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using System;

namespace SiegeDefense.GameComponents.Physics {
    public class GamePhysics : GameObject {
        public static Vector3 gravityForce { get; private set; } = new Vector3(0, -9.8f, 0);

        public Collider collider { get; set; }
        public float Mass { get; set; } = 1;
        public Vector3 Force { get; set; } = Vector3.Zero;
        public Vector3 Acceleration {
            get {
                return Force / Mass;
            }
        }
        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Map map { get; set; }

        public void AddForce(Vector3 force) {
            Force += force;
        }

        public override void GetDependentComponents() {
            map = (Map)FindObjectsByTag("Map")[0];
            collider = FindComponent<Collider>();
        }

        public override void Update(GameTime gameTime) {

            // update target position
            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            collider.Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // update force
            if (collider.GroundDistance(map) > 0) {
                Force += gravityForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else {
                Force = new Vector3(Force.X, 0, Force.Z);
            }
        }
    }
}
