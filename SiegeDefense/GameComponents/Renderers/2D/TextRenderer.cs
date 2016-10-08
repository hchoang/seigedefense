using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class TextRenderer : _2DRenderer {
        public SpriteFont font { get; set; }
        public string text { get; set; }
        
        public TextRenderer(string text, SpriteFont font) {
            this.text = text;
            this.font = font;
        }

        public TextRenderer(string text) : this(text, _game.Content.Load<SpriteFont>(@"Fonts\Arial")) { }

        public TextRenderer() : this("", _game.Content.Load<SpriteFont>(@"Fonts\Arial")) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {

            Vector2 drawPosition = position;
            if (parentRenderer != null) {
                Vector2 textSize = font.MeasureString(text);
                Rectangle parentDrawArea = parentRenderer.GetDrawArea();
                // middle align
                drawPosition = new Vector2(parentDrawArea.X + parentDrawArea.Width * position.X / 2 + parentDrawArea.Width / 2 - textSize.X / 2, 
                                       parentDrawArea.Y + parentDrawArea.Height * position.Y / 2 +  parentDrawArea.Height / 2 - textSize.Y / 2);
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, customRS);
            spriteBatch.DrawString(font, text, drawPosition, color, rotation, rotationOrigin, size, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }
    }
}
