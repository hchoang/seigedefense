using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class TankBullet : _3DGameObject
    {
        public int damage { get; private set; }
        public _3DGameObject owner { get; set; }
        public TankBullet(ModelType modelType, _3DGameObject owner)
        {
            this.damage = 20;
            this.owner = owner;

            Model bulletModel = Game.Content.Load<Model>(modelType.ToDescription());
            renderer = new ModelRenderer(bulletModel);
            AddComponent(renderer);

            collider = new Collider(bulletModel);
            AddComponent(collider);

            physics = new BulletPhysics();
            AddComponent(physics);
        }

        public override void Update(GameTime gameTime) {
            // collision checking with other tanks
            Tank collidedTank = null;
            foreach (Tank tank in FindObjects<Tank>()) {
                if (owner == tank) continue;

                if (collider.Intersect(tank.collider)) {
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
            if (!map.IsInsideMap(transformation.Position)) {
                Game.Components.Remove(this);
                return;
            }

            float bulletHeight = transformation.Position.Y;
            float mapHeight = map.GetHeight(transformation.Position);
            if (bulletHeight < mapHeight) {
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
