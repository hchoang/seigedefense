using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public class TitleMenu : GameObject {

        public SpriteRenderer frameRenderer { get; set; }

        public TitleMenu() {
            Game.Components.Add(this);

            // Load Frame
            frameRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame"));
            transformation.Position = new Vector3(0.7f, 0.2f, 0);
            transformation.ScaleMatrix = Matrix.CreateScale(0.3f, 0.5f, 0);
            AddComponent(frameRenderer);

            // Load buttons
            string[] buttonTexts = { "New Game", "Option", "Map Editor", "Exit" };
            Func<bool>[] buttonFuncs = { NewGame, Option, Option, Exit };
            Texture2D buttonFrame = Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton");
            SpriteFont buttonTextFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");

            float buttonWidth = 0.8f, buttonX = 0.1f;
            float buttonHeight = (1.0f / (buttonTexts.Length + 1)) * 0.75f;

            for (int i = 0; i < buttonTexts.Length; i++) {
                float buttonY = i * 1.0f / (buttonTexts.Length + 1) + 0.1f;
                Button button = new Button(buttonFrame, buttonTexts[i], buttonTextFont, buttonX, buttonY, buttonWidth, buttonHeight, frameRenderer.GetDrawArea());
                button.onClick = buttonFuncs[i];
                Game.Components.Add(button);
            }
        }

        public bool NewGame() {
            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadLevel("level1");
            return false;
        }

        public bool Exit() {
            Game.Exit();
            return false;
        }

        public bool Option() {
            return false;
        }
    }
}
