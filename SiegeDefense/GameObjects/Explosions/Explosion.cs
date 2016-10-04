using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class Explosion : _3DGameObject {

        private static List<Texture2D> explosionTextureList;
        public float changeTextureTime { get; set; } = 0.05f;
        public float changeTextureCounter { get; set; } = 0;
        public int textureIndex { get; set; } = 0;

        public new BillboardRenderer renderer { get; set; }

        public Explosion() {
            Texture2D explosionTexture = Game.Content.Load<Texture2D>(@"Sprites\Explosions");

            if (explosionTextureList == null) {
                Utility.GetSubTextures(explosionTexture, 16, 1, 0, 0, 16, 8, Game.GraphicsDevice, out explosionTextureList);
            }

            renderer = new BillboardRenderer(explosionTextureList[0]);
            AddComponent(renderer);
        }

        public override void Update(GameTime gameTime) {

            if (changeTextureCounter >= changeTextureTime) {
                changeTextureCounter = 0;
                textureIndex++;

                if (textureIndex == explosionTextureList.Count) {
                    Game.Components.Remove(this);
                } else {
                    renderer.texture = explosionTextureList[textureIndex];
                }

            }

            changeTextureCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}
