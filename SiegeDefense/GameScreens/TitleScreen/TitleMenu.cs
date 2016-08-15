using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense.GameComponents.TitleScreen {
    public class TitleMenu : GameObject {

        private TitleScreenBackground background = null;

        public TitleMenu() {
            // Load Frame
            Texture2D frameTexture = Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame");
            Vector2 framePosition = new Vector2(0.7f, 0.2f);
            Vector2 frameSize = new Vector2(0.3f, 0.5f);
            Rectangle frameDrawArea = Utility.CalculateDrawArea(framePosition, frameSize, 
                Game.Services.GetService<GraphicsDeviceManager>());
            Color frameColor = Color.White;
            AddChild(new Static2DSprite(frameTexture, frameDrawArea, frameColor));

            // Load buttons
            string[] buttonTexts = { "New Game", "Option", "Credit", "Exit" };
            double buttonWidth = 0.8, buttonX = 0.1;
            double buttonHeight = (1.0 / (buttonTexts.Length + 1)) * 0.75;

            for (int i = 0; i < buttonTexts.Length; i++) {
                double buttonY = i * 1.0 / (buttonTexts.Length + 1) + 0.1;
                AddChild(new TitleMenuButton(buttonTexts[i], buttonX, buttonY, buttonWidth, buttonHeight, frameDrawArea));
            }
        }

        public override void Draw(GameTime gameTime) {
            if (background == null) {
                background = FindComponent<TitleScreenBackground>();
                return;
            }

            if (!background.fadeInCompleted())
                return;

            base.Draw(gameTime);
        }
    }
}
