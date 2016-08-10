using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameObjects.Primitives {
    public class Quad : GameObject{
        private VertexPositionNormalTexture[] vertexList = new VertexPositionNormalTexture[4];
        private Texture2D texture;
        private static Vector2[] defaultTextureMapping = new Vector2[4] { new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0) };
        private BasicEffect effect;
        private VertexBuffer vertexBuffer;
        private int[] indices = new int[6];

        private GraphicsDevice graphicsDevice;
        private Camera camera;

        public Quad(Vector3[] data, Texture2D texture) {
            if (data.Length != 4)
                throw new Exception("Quad objects need 4 vertex");

            this.texture = texture;

            graphicsDevice = Game.Services.GetService<GraphicsDeviceManager>().GraphicsDevice;
            camera = Game.Services.GetService<Camera>();

            Vector2[] textureMapping = defaultTextureMapping;
            for (int i = 0; i < 4; i++) {
                vertexList[i] = new VertexPositionNormalTexture(data[i],
                    Vector3.Normalize(Vector3.Cross(data[i] - data[(i + 1) % 4], data[i] - data[(i + 2) % 4])),
                    textureMapping[i]);
            }

            effect = new BasicEffect(graphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = texture;
            

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), 4, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertexList);

            // first triangle indices
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            // second triangle indices
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
        }

        public override void Draw(GameTime gameTime) {
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            effect.World = WorldMatrix;
            GameObject ancestorPointer = ParentObject;
            while (ancestorPointer != null) {
                effect.World = ancestorPointer.WorldMatrix * effect.World;
                ancestorPointer = ancestorPointer.ParentObject;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexList, 0, 4, indices, 0, 2);
            }
        }
    }
}
