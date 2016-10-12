using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class TextBox : HUD {
        public Color backgroundColor { get; set; } = Color.Black;
        public Color textColor { get; set; } = Color.White;
        public SpriteFont font { get; set; }
        public SpriteRenderer backgroundRenderer { get; set; }
        public TextRenderer textRenderer { get; set; }
        public bool focused { get; set; } = false;
        public TextBox(Color backgroundColor, Color textColor, SpriteFont font, Vector2 position, Vector2 size) {
            this.backgroundColor = backgroundColor;
            this.textColor = textColor;
            this.font = font;

            backgroundRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\WhiteBar"));
            backgroundRenderer.color = backgroundColor;
            backgroundRenderer.position = position;
            backgroundRenderer.size = size;

            textRenderer = new TextRenderer();
            textRenderer.color = textColor;
            backgroundRenderer.AddChildRenderer(textRenderer);

            this.renderer = backgroundRenderer;
            AddComponent(backgroundRenderer);

            this.onClick = OnClick;
            this.onBlur = OnBlur;
        }

        public TextBox(Color backgroundColor, Color textColor, Vector2 position, Vector2 size) 
            : this(backgroundColor, textColor, _game.Content.Load<SpriteFont>(@"Fonts\Arial"), position, size) {
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (!focused)
                return;

            if (inputManager.isTriggered(GameInput.TextInput)) {
                float textInputValue = inputManager.GetValue(GameInput.TextInput);
                if (textInputValue != 0) {
                    textRenderer.text += (char)textInputValue;
                }
            }

            if (inputManager.isTriggered(GameInput.Back)) {
                if (textRenderer.text.Length > 0) {
                    textRenderer.text = textRenderer.text.Substring(0, textRenderer.text.Length - 1);
                }
            }

            if (inputManager.isTriggered(GameInput.NumberInput)) {
                float numberInputValue = inputManager.GetValue(GameInput.NumberInput);
                if (numberInputValue != 0) {
                    textRenderer.text += (char)numberInputValue;
                }
            }
        }

        public void OnClick(HUD invoker) {
            focused = true;
        }

        public void OnBlur(HUD invoker) {
            focused = false;
        }
    }
}
