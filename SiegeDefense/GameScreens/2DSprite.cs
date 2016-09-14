using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class _2DSprite : GameObject
    {
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            base.Update(gameTime);
        }
    }
}
