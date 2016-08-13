using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.Physics {
    public class GamePhysics : GameObject {
        private static Vector3 gravity = new Vector3(0, -9.8f, 0);

        public Vector3 Position { get; set; }
        private Vector3 Acceleration = Vector3.Zero;
        private Vector3 Velocity  = Vector3.Zero;
        public float Mass { get; set; } = 1;
        public MultiTexturedHeightMap map { get; set; }

        public void AddForce(Vector3 Force) {
            Acceleration = Force / Mass;
        }

        public override void Update(GameTime gameTime) {
            if (map.isOverGround(Position)) {
                Acceleration += gravity;
            }

            Velocity += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
