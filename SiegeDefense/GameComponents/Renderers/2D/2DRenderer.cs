using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public abstract class _2DRenderer : Renderer {

        private SpriteBatch _spriteBatch;

        protected SpriteBatch spriteBatch {
            get {
                if (_spriteBatch == null) {
                    _spriteBatch = Game.Services.GetService<SpriteBatch>();
                }
                return _spriteBatch;
            }
        }

        public sealed override void Draw(GameTime gameTime) {
            DepthStencilState oldDS = Game.GraphicsDevice.DepthStencilState;

            Draw(gameTime, spriteBatch);

            Game.GraphicsDevice.DepthStencilState = oldDS;

            base.Draw(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}
