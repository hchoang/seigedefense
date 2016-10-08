using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SiegeDefense {
    public class TitleScreen : GameObject {

        public TitleScreen() {
            _2DRenderer background = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuBackground"));
            AddComponent(background);

            _2DRenderer menuFrame = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame"));
            menuFrame.position = new Vector2(0.7f, 0.2f);
            menuFrame.size = new Vector2(0.3f, 0.5f);
            AddComponent(menuFrame);

            // load menu buttons
            string[] buttonTexts = { "New Game", "Option", "Map Editor", "Exit" };
            Action<HUD>[] buttonFuncs = { NewGame, Option, MapEditor, Exit };
            Texture2D buttonFrame = Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton");
            SpriteFont buttonTextFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");

            float buttonWidth = 0.8f, buttonX = 0.1f;
            float buttonHeight = (1.0f / (buttonTexts.Length + 1)) * 0.75f;
            Vector2 buttonSize = new Vector2(buttonWidth, buttonHeight);
            for (int i = 0; i < buttonTexts.Length; i++) {
                float buttonY = i * 1.0f / (buttonTexts.Length + 1) + 0.1f;

                _2DRenderer buttonRenderer = new SpriteRenderer(buttonFrame);
                Vector2 buttonPosition = new Vector2(buttonX, buttonY);
                buttonRenderer.position = buttonPosition;
                buttonRenderer.size = buttonSize;
                buttonRenderer.AddChildRenderer(new TextRenderer(buttonTexts[i]));

                menuFrame.AddChildRenderer(buttonRenderer);
                
                HUD button = new HUD(buttonRenderer);
                button.onClick = buttonFuncs[i];
                AddComponent(button);
            }
        }

        public void NewGame(HUD invoker) {
            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadLevel("level1");
        }

        public void Exit(HUD invoker) {
            Game.Exit();
        }

        public void MapEditor(HUD invoker) {
            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadMapEditorSelectionScreen();
        }

        public void Option(HUD invoker) {

        }
    }
}
