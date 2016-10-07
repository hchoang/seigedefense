using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class SpriteRenderer : _2DRenderer {
        public Texture2D sprite { get; set; }
        public Color color { get; set; } = Color.White;
        public Rectangle? root { get; set; }

        public SpriteRenderer(Texture2D sprite) {
            this.sprite = sprite;
        }

        public Rectangle GetDrawArea() {
            Rectangle drawArea = new Rectangle();
            Vector2 position = baseObject.transformation.Position.ToVector2();
            Vector2 size = baseObject.transformation.ScaleMatrix.Scale.ToVector2();

            if (root == null) {
                drawArea = Utility.CalculateDrawArea(position, size, Game.GraphicsDevice);
            } else {
                drawArea = Utility.CalculateDrawArea(position, size, (Rectangle)root);
            }

            return drawArea;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            Rectangle drawArea = GetDrawArea();

            spriteBatch.Begin();
            spriteBatch.Draw(sprite, drawArea, color);
            spriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }
    }
}
