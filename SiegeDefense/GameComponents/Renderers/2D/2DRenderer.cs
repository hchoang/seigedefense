using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class _2DRenderer : Renderer {

        private SpriteBatch _spriteBatch;

        public SpriteBatch spriteBatch {
            get {
                if (_spriteBatch == null) {
                    _spriteBatch = Game.Services.GetService<SpriteBatch>();
                }
                return _spriteBatch;
            }
        }

        public virtual Vector2 position { get; set; } = Vector2.Zero;
        public virtual Vector2 size { get; set; } = Vector2.One;
        public virtual Color color { get; set; } = Color.White;
        public virtual float rotation { get; set; } = 0;
        public virtual Vector2 rotationOrigin { get; set; } = Vector2.Zero;
        public virtual _2DRenderer parentRenderer { get; set; }
        public virtual List<_2DRenderer> childRenderers { get; set; } = new List<_2DRenderer>();
        public virtual Rectangle drawArea { get; set; }
        public RasterizerState customRS { get; set; }
        public Rectangle? scissorRect { get; set; }

        public void SetRasterizerState(RasterizerState rs, bool recursive) {
            customRS = rs;
            if (recursive) {
                foreach (_2DRenderer childRenderer in childRenderers) {
                    childRenderer.SetRasterizerState(rs, true);
                }
            }
        }

        public void SetScissorRectangle(Rectangle rect, bool recursive) {
            scissorRect = rect;
            if (recursive) {
                foreach(_2DRenderer childRenderer in childRenderers) {
                    childRenderer.SetScissorRectangle(rect, true);
                }
            }
        }

        public Rectangle GetDrawArea() {

            if (parentRenderer == null) {
                drawArea = Utility.CalculateDrawArea(position, size, Game.GraphicsDevice);
            } else {
                drawArea = Utility.CalculateDrawArea(position, size, parentRenderer.GetDrawArea());
            }

            return drawArea;
        }

        public void AddChildRenderer(_2DRenderer childRenderer) {
            childRenderers.Add(childRenderer);
            childRenderer.parentRenderer = this;
        }

        public sealed override void Draw(GameTime gameTime) {
            DepthStencilState oldDS = Game.GraphicsDevice.DepthStencilState;
            

            Draw(gameTime, spriteBatch);

            Game.GraphicsDevice.DepthStencilState = oldDS;

            base.Draw(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            if (scissorRect != null) {
                spriteBatch.GraphicsDevice.ScissorRectangle = (Rectangle)scissorRect;
            }
            foreach (_2DRenderer renderer in childRenderers) {
                renderer.Draw(gameTime, spriteBatch);
            }
        }
    }
}
