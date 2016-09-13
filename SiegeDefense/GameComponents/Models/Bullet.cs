using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Models
{
    public class Bullet : BaseModel
    {
        public int damage { get; private set; }
        public BaseModel owner { get; set; }
        public Bullet(ModelType modelType, BaseModel owner): base(modelType)
        {
            this.damage = 20;
            this.owner = owner;
        }

        public override void Update(GameTime gameTime) {
            // collision checking with other tanks
            Tank collidedTank = null;
            foreach (Tank tank in FindObjects<Tank>()) {
                if (owner == tank) continue;

                if (collisionBox.Intersect(tank.collisionBox)) {
                    //Console.Out.WriteLine("Intersect");
                    collidedTank = tank;
                    break;
                }
            }

            if (collidedTank != null) {
                collidedTank.Damaged(this);
                Game.Components.Remove(this);
            }

            // remove bullet if outside map or hit the ground
            if (!map.IsInsideMap(Position)) {
                Game.Components.Remove(this);
                return;
            }

            float bulletHeight = Position.Y;
            float mapHeight = map.GetHeight(Position);
            if (bulletHeight < mapHeight) {
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
