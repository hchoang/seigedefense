using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SiegeDefense {
    public class HPRestoreItem : Item {
        public HPRestoreItem() {
            renderer = new BillboardRenderer(Game.Content.Load<Texture2D>(@"Sprites\HPRestore"));
            AddComponent(renderer);

            collider = new Collider(new BoundingBox(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 1, 0.5f)));
            AddComponent(collider);
        }

        public override void Update(GameTime gameTime) {
            List <OnlandVehicle> tankList = FindObjectsInPartition<OnlandVehicle>();
            OnlandVehicle targetTank = null;
            foreach (OnlandVehicle tank in tankList) {
                if (tank.collider.Intersect(this.collider)) {
                    targetTank = tank;
                    break;
                }
            }

            if (targetTank != null) {
                targetTank.HP += 10;
                //Game.Components.Remove(this);
                RemoveFromGameWorld();
            }
        }
    }
}
