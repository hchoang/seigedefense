using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class BulletPhysics : GamePhysics {
        public override void Update(GameTime gameTime) {
            Vector3 forward = baseObject.transformation.Forward;
            forward.Normalize();

            baseObject.transformation.Position += forward * (float)gameTime.ElapsedGameTime.TotalSeconds * 1000;
        }
    }
}
