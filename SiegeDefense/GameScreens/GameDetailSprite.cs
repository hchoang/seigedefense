using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public class GameDetailSprite : _2DSprite
    {
        private SpriteBatch spriteBatch;

        private SpriteFont font;
        private Vector2 position;
        private Color color;
        private String text;

        public GameDetailSprite(SpriteFont font, String text, Vector2 position, Color color)
        {
            spriteBatch = Game.Services.GetService<SpriteBatch>();

            this.font = font;
            this.position = position;
            this.color = color;
            this.text = text;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, position, color);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, position, color);
            spriteBatch.End();
            base.Update(gameTime);
        }

        public void setText(String text)
        {
            this.text = text;
        }
    }
}
