using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class SpriteRenderer : _2DRenderer {
        public Texture2D sprite { get; set; }

        public SpriteRenderer(Texture2D sprite, Vector2 position, Vector2 size) {
            this.sprite = sprite;
            this.position = position;
            this.size = size;
        }

        public SpriteRenderer(Texture2D sprite) : this(sprite, Vector2.Zero, Vector2.One) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            Rectangle drawArea = GetDrawArea();

            spriteBatch.Begin();
            spriteBatch.Draw(sprite, drawArea, null, color, rotation, rotationOrigin, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }
    }
}
