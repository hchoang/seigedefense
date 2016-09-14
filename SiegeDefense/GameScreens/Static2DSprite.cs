using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense
{
     public class Static2DSprite : _2DSprite {
        private SpriteBatch spriteBatch;

        private Texture2D texture;
        private Rectangle drawArea;
        private Color color;

        public Static2DSprite(Texture2D texture, Rectangle drawArea, Color color) {
            spriteBatch = Game.Services.GetService<SpriteBatch>();

            this.texture = texture;
            this.drawArea = drawArea;
            this.color = color;
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, drawArea, color);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
