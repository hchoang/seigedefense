using SiegeDefense.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense.GameScreens
{
    class _2DSprite : GameObject
    {
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        }
    }
}
