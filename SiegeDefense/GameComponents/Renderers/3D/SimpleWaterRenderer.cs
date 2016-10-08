using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class SimpleWaterRenderer : WaterRenderer {
        public SimpleWaterRenderer(Vector2 corner1, Vector2 corner2, float waterHeight) : base(corner1, corner2, waterHeight) {
        }

        public override void Draw(GameTime gameTime) {
            DrawReflectionMap(gameTime);
            DrawWater(gameTime);
            base.Draw(gameTime);
        }
    }
}
