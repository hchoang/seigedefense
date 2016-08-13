using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense.GameComponents.TitleScreen {
    public class TitleScreenBackground : GameObject {

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D backgroundImage;
        private Rectangle drawArea;
        private Color color = Color.Black;
        private double fadeInTime = 2;
        private double fadeInCounter = 0;
        
        public TitleScreenBackground() {
            backgroundImage = Game.Content.Load<Texture2D>(@"Sprites\MainMenuBackground");

            graphics = Game.Services.GetService<GraphicsDeviceManager>();
            drawArea = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            spriteBatch = Game.Services.GetService<SpriteBatch>();
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundImage, drawArea, color);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime) {

            if (fadeInCounter < fadeInTime) {
                color.R = color.G = color.B = (byte)( fadeInCounter * 256 / fadeInTime);
                fadeInCounter += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public bool fadeInCompleted() {
            return fadeInCounter >= fadeInTime;
        }
    }
}
