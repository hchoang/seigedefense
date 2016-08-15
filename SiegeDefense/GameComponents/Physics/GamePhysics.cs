using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using System;

namespace SiegeDefense.GameComponents.Physics {
    public class GamePhysics : GameObject {
        protected static Vector3 gravityForce = new Vector3(0, -9.8f, 0);

        public CollisionBoundary Boundary { get; set; }
        public _3DGameObject Target { get; set; }
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

        public override void Update(GameTime gameTime) {

            // update target position
            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Boundary.TranslationMatrix *= Matrix.CreateTranslation(Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            // update force
            if (Boundary.GroundDistance(map) > 0) {
                Force += gravityForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else {
                Force = new Vector3(Force.X, 0, Force.Z);
            }

            Vector3 groundNormal = map.GetNormal(Target.Position);
        }
    }
}
