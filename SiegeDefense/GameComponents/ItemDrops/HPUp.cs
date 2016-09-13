using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Models;

namespace SiegeDefense.GameComponents.ItemDrops {
    public class HPUp : ItemDrop {
        public HPUp() {
            texture = Game.Content.Load<Texture2D>(@"Sprites\HPRestore");
        }

        public override void Update(GameTime gameTime) {
            List <Tank> tankList = FindObjects<Tank>();
            Tank targetTank = null;
            foreach (Tank tank in tankList) {
                if (tank.collisionBox.Intersect(collisionBox)) {
                    targetTank = tank;
                    break;
                }
            }

            if (targetTank != null) {
                targetTank.HP += 10;
                Game.Components.Remove(this);
            }
        }
    }
}
