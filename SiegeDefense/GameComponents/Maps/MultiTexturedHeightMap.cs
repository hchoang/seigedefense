using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using System;

namespace SiegeDefense.GameComponents.Maps {
    public class MultiTexturedHeightMap : Map {
        private Texture2D sandTexture;
        private Texture2D grassTexture;
        private Texture2D rockTexture;
        private Texture2D snowTexture;
        
        private Effect advancedEffect;

        // Map attributes
        private int mapInfoWidth;
        private int mapInfoHeight;
        private float minMapHeight = float.MaxValue;
        private float maxMapHeight = float.MinValue;

        // Map configs - will be configued via constructor/setter
        private float mapCellSize = 10.0f;
        private float mapDeltaHeight = 100.0f; // max height - min height

        // Map texture weight
        private float minSandHeight = -0.3f;
        private float maxSandHeight = 0.3f;
        private float minGrassHeight = 0.25f;
        private float maxGrassHeight = 0.6f;
        private float minRockHeight = 0.55f;
        private float maxRockHeight = 0.85f;
        private float minSnowHeight = 0.8f;
        private float maxSnowHeight = 1.2f;

        // Map data
        private float[,] heightInfo;
        VertexMultiTextured[] vertices;
        private int[] vertexIndices;

        public MultiTexturedHeightMap(float mapCellSize, float mapDeltaHeight) {
            this.mapCellSize = mapCellSize;
            this.mapDeltaHeight = mapDeltaHeight;

            sandTexture = Game.Content.Load<Texture2D>(@"Terrain\sand");
            grassTexture = Game.Content.Load<Texture2D>(@"Terrain\grass");
            rockTexture = Game.Content.Load<Texture2D>(@"Terrain\rock");
            snowTexture = Game.Content.Load<Texture2D>(@"Terrain\snow");

            advancedEffect = Game.Services.GetService<Effect>();
            advancedEffect.Parameters["EnableLighting"].SetValue(true);
            advancedEffect.Parameters["Ambient"].SetValue(0.4f);
            advancedEffect.Parameters["LightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            ReadTerrainFromTexture();
        }        

        public override void Draw(GameTime gameTime) {
            advancedEffect.CurrentTechnique = advancedEffect.Techniques["MultiTextured"];
            advancedEffect.Parameters["World"].SetValue(WorldMatrix);
            advancedEffect.Parameters["xTexture0"].SetValue(sandTexture);
            advancedEffect.Parameters["xTexture1"].SetValue(grassTexture);
            advancedEffect.Parameters["xTexture2"].SetValue(rockTexture);
            advancedEffect.Parameters["xTexture3"].SetValue(snowTexture);
            foreach (EffectPass pass in advancedEffect.CurrentTechnique.Passes) {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, vertexIndices, 0, vertexIndices.Length / 3);
            }
        }

        public override bool Moveable(Vector3 position) {
            Vector3 firstVertexPosition = vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int gridMapPositionX = (int)(relativePosition.X / mapCellSize);
            int gridMapPositionY = (int)(relativePosition.Z / mapCellSize);

            if (gridMapPositionX < 0 || gridMapPositionX > mapInfoWidth) return false;
            if (gridMapPositionY < 0 || gridMapPositionY > mapInfoHeight) return false;

            return true;
        }
        
        public override float GetHeight(Vector3 position) {
            Vector3 firstVertexPosition = vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int gridMapPositionX = (int)(relativePosition.X / mapCellSize);
            int gridMapPositionY = (int)(relativePosition.Z / mapCellSize);

            float cellPositionX = relativePosition.X % mapCellSize / mapCellSize;
            float cellPositionY = relativePosition.Z % mapCellSize / mapCellSize;

            float h1 = vertices[gridMapPositionX + gridMapPositionY * mapInfoWidth].Position.Y;
            float h2 = vertices[gridMapPositionX + 1 + gridMapPositionY * mapInfoWidth].Position.Y;
            float h3 = vertices[gridMapPositionX + (gridMapPositionY + 1) * mapInfoWidth].Position.Y;
            float h4 = vertices[gridMapPositionX + 1 + (gridMapPositionY + 1) * mapInfoWidth].Position.Y;

            float h12 = MathHelper.Lerp(h1, h2, cellPositionX);
            float h34 = MathHelper.Lerp(h3, h4, cellPositionX);

            float height = MathHelper.Lerp(h12, h34, cellPositionY);

            return height;
        }

        public override Vector3 GetNormal(Vector3 position) {
            Vector3 firstVertexPosition = vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int gridMapPositionX = (int)(relativePosition.X / mapCellSize);
            int gridMapPositionY = (int)(relativePosition.Z / mapCellSize);

            float cellPositionX = relativePosition.X % mapCellSize / mapCellSize;
            float cellPositionY = relativePosition.Z % mapCellSize / mapCellSize;

            Vector3 v1 = vertices[gridMapPositionX + gridMapPositionY * mapInfoWidth].Normal;
            Vector3 v2 = vertices[gridMapPositionX + 1 + gridMapPositionY * mapInfoWidth].Normal;
            Vector3 v3 = vertices[gridMapPositionX + (gridMapPositionY + 1) * mapInfoWidth].Normal;
            Vector3 v4 = vertices[gridMapPositionX + 1 + (gridMapPositionY + 1) * mapInfoWidth].Normal;

            Vector3 v12 = Vector3.Lerp(v1, v2, cellPositionX);
            Vector3 v34 = Vector3.Lerp(v3, v4, cellPositionX);

            Vector3 normal = Vector3.Lerp(v12, v34, cellPositionY);

            return normal;
        }

        private void ReadTerrainFromTexture() {
            // Load texture & read metadata
            Texture2D heightMap = Game.Content.Load<Texture2D>(@"Terrain\heightmap");
            mapInfoWidth = heightMap.Width;
            mapInfoHeight = heightMap.Height;

            Color[] heightMapColors = new Color[mapInfoWidth * mapInfoHeight];
            heightMap.GetData(heightMapColors);

            heightInfo = new float[mapInfoWidth, mapInfoHeight];

            // Read texture data into height data
            for (int x=0; x<mapInfoWidth; x++) {
                for (int y=0; y<mapInfoHeight; y++) {
                    heightInfo[x, y] = heightMapColors[x + y * mapInfoWidth].R;

                    if (heightInfo[x, y] < minMapHeight) minMapHeight = heightInfo[x, y];
                    if (heightInfo[x, y] > maxMapHeight) maxMapHeight = heightInfo[x, y];
                }
            }

            // normalize height data to [0-mapHeighMax]
            for (int x = 0; x < mapInfoWidth; x++) {
                for (int y = 0; y < mapInfoHeight; y++) {
                    heightInfo[x, y] = (heightInfo[x, y] - minMapHeight) / (maxMapHeight - minMapHeight) * mapDeltaHeight;
                }
            }

            // prepare data for calculating texture weight
            float midSandHeight = (minSandHeight + maxSandHeight) / 2 * mapDeltaHeight;
            float deltaSandHeight = (maxSandHeight - minSandHeight) / 2 * mapDeltaHeight;
            float midGrassHeight = (minGrassHeight + maxGrassHeight) / 2 * mapDeltaHeight;
            float deltaGrassHeight = (maxGrassHeight - minGrassHeight) / 2 * mapDeltaHeight;
            float midRockHeight = (minRockHeight + maxRockHeight) / 2 * mapDeltaHeight;
            float deltaRockHeight = (maxRockHeight - minRockHeight) / 2 * mapDeltaHeight;
            float midSnowHeight = (minSnowHeight + maxSnowHeight) / 2 * mapDeltaHeight;
            float deltaSnowHeight = (maxSnowHeight - minSnowHeight) / 2 * mapDeltaHeight;

            // parse height data to vertices
            vertices = new VertexMultiTextured[mapInfoWidth * mapInfoHeight];
            for (int x=0; x<mapInfoWidth; x++) {
                for (int y=0; y<mapInfoHeight; y++) {
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
            vertexIndices = new int[(mapInfoWidth-1) * (mapInfoHeight-1) * 6];
            int indiceCounter = 0;
            for (int y=0; y<mapInfoHeight-1; y++) {
                for (int x=0; x<mapInfoWidth-1; x++) {
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

            for (int i=0; i<vertexIndices.Length / 3; i++) {
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
            for (int i=0; i < vertices.Length; i++) {
                vertices[i].Normal.Normalize();
            }
        }
    }
}
