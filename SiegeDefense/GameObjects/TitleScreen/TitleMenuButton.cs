using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense.GameObjects.TitleScreen {
    public class TitleMenuButton : GameObject {
        private SpriteBatch spriteBatch;
        
        private Texture2D buttonFrameTexture;
        private Rectangle frameDrawArea;
        private Color frameColor = Color.White;
        private double frameOpacity = 0.8;
        private double frameOpacityController = 2;

        private SpriteFont textFont;
        private Vector2 textPosition;
        private Color textColor = Color.White;
        private string text;

        public TitleMenuButton(string text, double x, double y, double width, double height, Rectangle root) {
            spriteBatch = Game.Services.GetService<SpriteBatch>();
            
            buttonFrameTexture = Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton");
            frameDrawArea = Utility.CalculateDrawArea(x, y, width, height, root);

            this.text = text;
            textFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");
            Vector2 textSize = textFont.MeasureString(text);
            textPosition = new Vector2(frameDrawArea.X + (frameDrawArea.Width - textSize.X) / 2,
                frameDrawArea.Y + (frameDrawArea.Height - textSize.Y) / 2);
        }

        public override void Draw(GameTime gameTime) {
            spriteBatch.Begin();
            spriteBatch.Draw(buttonFrameTexture, frameDrawArea, frameColor);
            spriteBatch.DrawString(textFont, text, textPosition, textColor);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime) {
        }
    }
}
