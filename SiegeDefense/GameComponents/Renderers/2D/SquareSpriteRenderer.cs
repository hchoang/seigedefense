using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class SquareSpriteRenderer : SpriteRenderer {
        public Texture2D paddingTexture { get; set; }
        public SquareSpriteRenderer(Texture2D sprite, Color paddingColor) : base(sprite) {
            paddingTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            paddingTexture.SetData(new Color[] { paddingColor });
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            drawArea = GetDrawArea();

            Rectangle paddingDrawArea = drawArea;
            paddingDrawArea.Width = MathHelper.Max(paddingDrawArea.Width, paddingDrawArea.Height);
            paddingDrawArea.Height = paddingDrawArea.Width;

            Rectangle spriteDrawArea = paddingDrawArea;
            if (sprite.Width > sprite.Height) {
                float ratio = sprite.Height * 1.0f / sprite.Width;
                spriteDrawArea.Height = (int)(paddingDrawArea.Height * ratio);
                spriteDrawArea.Y += (int)(paddingDrawArea.Height * (0.5f - (ratio / 2)));
            } else {
                float ratio = sprite.Width * 1.0f / sprite.Height;
                spriteDrawArea.Width = (int)(paddingDrawArea.Width * ratio);
                spriteDrawArea.X += (int)(paddingDrawArea.Width * (0.5f - (ratio / 2)));
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, customRS);
            spriteBatch.Draw(paddingTexture, paddingDrawArea, Color.White);
            spriteBatch.Draw(sprite, spriteDrawArea, color);
            spriteBatch.End();
        }
    }
}
