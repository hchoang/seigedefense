using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public class Button : GameObject {
        private IInputManager inputManager;

        public SpriteRenderer frameRenderer { get; set; }
        public TextRenderer textRenderer { get; set; }

        public Func<bool> onClick { get; set; }

        public Button(Texture2D frameSprite, string text, SpriteFont textFont, float x, float y, float width, float height, Rectangle root) {
            inputManager = Game.Services.GetService<IInputManager>();

            // frame
            transformation.Position = new Vector3(x, y, 0);
            transformation.ScaleMatrix = Matrix.CreateScale(width, height, 0);
            frameRenderer = new SpriteRenderer(frameSprite);
            frameRenderer.root = root;
            AddComponent(frameRenderer);
            Rectangle frameDrawArea = frameRenderer.GetDrawArea();

            // text
            Vector2 textSize = textFont.MeasureString(text);
            Vector2 textPosition = new Vector2(frameDrawArea.X + (frameDrawArea.Width - textSize.X) / 2,
                frameDrawArea.Y + (frameDrawArea.Height - textSize.Y) / 2);
            textRenderer = new TextRenderer();
            textRenderer.text = text;
            textRenderer.font = textFont;
            textRenderer.position = textPosition;
            AddComponent(textRenderer);
        }

        public override void Update(GameTime gameTime) {
            float x = inputManager.GetValue(GameInput.PointerX);
            float y = inputManager.GetValue(GameInput.PointerY);

            if (inputManager.isTriggered(GameInput.Fire) && frameRenderer.GetDrawArea().Contains(x, y)) {
                onClick();
            }

            base.Update(gameTime);
        }
    }
}
