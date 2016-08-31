using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Physics;
using System;

namespace SiegeDefense.GameComponents.Models
{
    class Bullet : BaseModel
    {
        protected int damage;
        public BaseModel owner { get; set; }
        public Bullet(Model model, BaseModel owner): base(model)
        {
            this.damage = 20;
            this.owner = owner;
        }

        public override void Update(GameTime gameTime) {
            // collision checking with other tanks
            Tank collidedTank = null;
            foreach (Tank tank in modelManager.getTankList()) {
                //if (Tag.Equals(tank.Tag)) continue;
                if (owner == tank) continue;

                if (collisionBox.Intersect(tank.collisionBox)) {
                    Console.Out.WriteLine("Intersect");
                    collidedTank = tank;
                    break;
                }
            }

            if (collidedTank != null) {
                collidedTank.Damaged(damage);
                modelManager.Remove(this);
            }

            // remove bullet if outside map or hit the ground
            if (!map.IsInsideMap(Position)) {
                modelManager.Remove(this);
                return;
            }

            float bulletHeight = Position.Y;
            float mapHeight = map.GetHeight(Position);
            if (bulletHeight < mapHeight) {
                modelManager.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
