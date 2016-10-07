using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public class HeightMapRenderer : _3DRenderer {
        // Texture & weights
        private Texture2D sandTexture;
        private Texture2D grassTexture;
        private Texture2D rockTexture;
        private Texture2D snowTexture;

        private float minSandHeight = -0.3f;
        private float maxSandHeight = 0.3f;
        private float minGrassHeight = 0.25f;
        private float maxGrassHeight = 0.6f;
        private float minRockHeight = 0.55f;
        private float maxRockHeight = 0.85f;
        private float minSnowHeight = 0.8f;
        private float maxSnowHeight = 1.2f;

        public VertexMultiTextured[] vertices { get; protected set; }
        private int[] vertexIndices;

        public HeightMapRenderer(Texture2D heightMap, float mapDeltaHeight, float mapCellSize, out int mapInfoWidth, out int mapInfoHeight) {
            customEffect.CurrentTechnique = customEffect.Techniques["MultiTextured"];
            customEffect.Parameters["EnableLighting"].SetValue(true);
            customEffect.Parameters["Ambient"].SetValue(0.4f);
            customEffect.Parameters["LightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            sandTexture = Game.Content.Load<Texture2D>(@"Terrain\sand");
            grassTexture = Game.Content.Load<Texture2D>(@"Terrain\grass");
            rockTexture = Game.Content.Load<Texture2D>(@"Terrain\rock");
            snowTexture = Game.Content.Load<Texture2D>(@"Terrain\snow");

            mapInfoWidth = heightMap.Width;
            mapInfoHeight = heightMap.Height;

            Color[] heightMapColors = new Color[mapInfoWidth * mapInfoHeight];
            heightMap.GetData(heightMapColors);

            float[,] heightInfo = new float[mapInfoWidth, mapInfoHeight];

            for (int x = 0; x < mapInfoWidth; x++) {
                for (int y = 0; y < mapInfoHeight; y++) {
                    heightInfo[x, y] = heightMapColors[x + y * mapInfoWidth].R;
                }
            }

            for (int x = 0; x < mapInfoWidth; x++) {
                for (int y = 0; y < mapInfoHeight; y++) {
                    heightInfo[x, y] = heightInfo[x, y] / 255 * mapDeltaHeight;
                }
            }

            float midSandHeight = (minSandHeight + maxSandHeight) / 2 * mapDeltaHeight;
            float deltaSandHeight = (maxSandHeight - minSandHeight) / 2 * mapDeltaHeight;
            float midGrassHeight = (minGrassHeight + maxGrassHeight) / 2 * mapDeltaHeight;
            float deltaGrassHeight = (maxGrassHeight - minGrassHeight) / 2 * mapDeltaHeight;
            float midRockHeight = (minRockHeight + maxRockHeight) / 2 * mapDeltaHeight;
            float deltaRockHeight = (maxRockHeight - minRockHeight) / 2 * mapDeltaHeight;
            float midSnowHeight = (minSnowHeight + maxSnowHeight) / 2 * mapDeltaHeight;
            float deltaSnowHeight = (maxSnowHeight - minSnowHeight) / 2 * mapDeltaHeight;

            vertices = new VertexMultiTextured[mapInfoWidth * mapInfoHeight];
            for (int x = 0; x < mapInfoWidth; x++) {
                for (int y = 0; y < mapInfoHeight; y++) {
                    vertices[x + y * mapInfoWidth].Position = new Vector3(x * mapCellSize, heightInfo[x, y], y * mapCellSize);
                    vertices[x + y * mapInfoWidth].TextureCoordinate.X = x * mapCellSize / mapDeltaHeight;
                    vertices[x + y * mapInfoWidth].TextureCoordinate.Y = y * mapCellSize / mapDeltaHeight;

                    // calculate texture weight
                    // X for sand, Y for grass, Z for rock, W for snow
                    vertices[x + y * mapInfoWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(heightInfo[x, y] - midSandHeight) / deltaSandHeight, 0, 1);
                    vertices[x + y * mapInfoWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(heightInfo[x, y] - midGrassHeight) / deltaGrassHeight, 0, 1);
                    vertices[x + y * mapInfoWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(heightInfo[x, y] - midRockHeight) / deltaRockHeight, 0, 1);
                    vertices[x + y * mapInfoWidth].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(heightInfo[x, y] - midSnowHeight) / deltaSnowHeight, 0, 1);

                    // normalize texture weight
                    float total = vertices[x + y * mapInfoWidth].TexWeights.X;
                    total += vertices[x + y * mapInfoWidth].TexWeights.Y;
                    total += vertices[x + y * mapInfoWidth].TexWeights.Z;
                    total += vertices[x + y * mapInfoWidth].TexWeights.W;

                    vertices[x + y * mapInfoWidth].TexWeights.X /= total;
                    vertices[x + y * mapInfoWidth].TexWeights.Y /= total;
                    vertices[x + y * mapInfoWidth].TexWeights.Z /= total;
                    vertices[x + y * mapInfoWidth].TexWeights.W /= total;
                }
            }

            // setup indices
            vertexIndices = new int[(mapInfoWidth - 1) * (mapInfoHeight - 1) * 6];
            int indiceCounter = 0;
            for (int y = 0; y < mapInfoHeight - 1; y++) {
                for (int x = 0; x < mapInfoWidth - 1; x++) {
                    int lowerLeft = x + y * mapInfoWidth;
                    int lowerRight = (x + 1) + y * mapInfoWidth;
                    int topLeft = x + (y + 1) * mapInfoWidth;
                    int topRight = (x + 1) + (y + 1) * mapInfoWidth;

                    // indices for first triangle
                    vertexIndices[indiceCounter++] = lowerLeft;
                    vertexIndices[indiceCounter++] = lowerRight;
                    vertexIndices[indiceCounter++] = topLeft;
                    // indices for second triangle
                    vertexIndices[indiceCounter++] = lowerRight;
                    vertexIndices[indiceCounter++] = topRight;
                    vertexIndices[indiceCounter++] = topLeft;
                }
            }

            // calculate normal vectors
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Normal = Vector3.Zero;
            }

            for (int i = 0; i < vertexIndices.Length / 3; i++) {
                int index1 = vertexIndices[i * 3];
                int index2 = vertexIndices[i * 3 + 1];
                int index3 = vertexIndices[i * 3 + 2];

                Vector3 v1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 v2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(v1, v2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            // normalize normal vectors
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Normal.Normalize();
            }
        }

        public void DrawMap(Effect effect, Matrix viewMatrix) {
            effect.Parameters["World"].SetValue(transformation.WorldMatrix);
            effect.Parameters["View"].SetValue(viewMatrix);
            effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            effect.Parameters["xTexture0"].SetValue(sandTexture);
            effect.Parameters["xTexture1"].SetValue(grassTexture);
            effect.Parameters["xTexture2"].SetValue(rockTexture);
            effect.Parameters["xTexture3"].SetValue(snowTexture);
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, vertexIndices, 0, vertexIndices.Length / 3);
            }
        }

        public override void Draw(GameTime gameTime) {
            DrawMap(customEffect, camera.ViewMatrix);

            base.Draw(gameTime);
        }
    }
}
