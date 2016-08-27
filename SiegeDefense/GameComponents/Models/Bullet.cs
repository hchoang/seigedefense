﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Physics;
using System;

namespace SiegeDefense.GameComponents.Models
{
    class Bullet : BaseModel
    {
        public Bullet(Model model, Vector3 Position): base(model, Position)
        {
        }

        public override void Update(GameTime gameTime) {
            // collision checking with other tanks
            Tank collidedTank = null;
            foreach (Tank tank in modelManager.tankList) {
                if (Tag.Equals(tank.Tag)) continue;

                if (collisionBox.Instersect(tank.collisionBox)) {
                    Console.Out.WriteLine("Intersect");
                    collidedTank = tank;
                    break;
                }
            }

            if (collidedTank != null) {
                collidedTank.Destroy();
                modelManager.models.Remove(this);
            }

            // remove bullet if outside map or hit the ground
            if (!map.IsInsideMap(Position)) {
                modelManager.models.Remove(this);
                return;
            }

            float bulletHeight = Position.Y;
            float mapHeight = map.GetHeight(Position);
            if (bulletHeight < mapHeight) {
                modelManager.models.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
