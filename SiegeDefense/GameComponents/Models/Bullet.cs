using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Maps;
using System;

namespace SiegeDefense.GameComponents.Models
{
    class Bullet : BaseModel
    {
        public Bullet(Model model, Vector3 Position): base(model, Position)
        {
        }

        public override void Update(GameTime gameTime) {
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
