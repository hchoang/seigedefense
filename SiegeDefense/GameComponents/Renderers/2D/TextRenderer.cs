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
        public Vector2 position;
        public Color color { get; set; } = Color.White;
        public string text { get; set; }
        public float rotation { get; set; } = 0;
        public Vector2 rotationOrigin { get; set; } = Vector2.Zero;
        public Vector2 scale { get; set; } = new Vector2(1, 1);
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, position, color, rotation, rotationOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }

        public Vector2 textSize {
            get {
                return (font.MeasureString(text));
            }
        }
    }
}
