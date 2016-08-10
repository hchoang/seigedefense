using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameObjects.Primitives;
using System;

namespace SiegeDefense.GameObjects.Map {
    public class HeightMap : GameObject {

        private float[,] heightInfo;
        private Vector3[,] normalVectorInfo;
        private float cellSize = 1;
        private float mapWidth;
        private float mapHeight;
        private Vector3 mapCenterPosition = Vector3.Zero;
        private Texture2D cellTexture;

        private Camera camera;
        private VertexPositionNormalTexture[] vertices;
        private int[] vertexIndices;
        private VertexBuffer vertexBuffer;
        private BasicEffect effect;

        public static HeightMap createFromTexture(Texture2D heightMapTexture, float cellSize) {

            int textureWidth = heightMapTexture.Width;
            int textureHeight = heightMapTexture.Height;
            Color[] heightMapColors = new Color[textureWidth * textureHeight];
            heightMapTexture.GetData(heightMapColors);

            float[,] heightInfo = new float[textureWidth, textureHeight];
            Vector3[,] normalVectorInfo = new Vector3[textureWidth, textureHeight];
            for (int x=0; x<textureWidth; x++) {
                for (int y=0; y<textureHeight; y++) {
                    heightInfo[x, y] = heightMapColors[x + y * textureWidth].R / 5.0f;
                }
            }

            return new HeightMap(heightInfo, cellSize);
        }

        public HeightMap(float[,] heightInfo, float cellSize) {
            if (null == heightInfo) throw new ArgumentNullException("heightInfo");

            this.heightInfo = heightInfo;

            int mapInfoWidth = heightInfo.GetLength(0);
            int mapInfoHeight = heightInfo.GetLength(1);

            mapWidth = (mapInfoWidth - 1) * cellSize;
            mapHeight = (mapInfoHeight - 1) * cellSize;

            mapCenterPosition.X = -(mapInfoWidth - 1) / 2.0f * cellSize;
            mapCenterPosition.Y = 0;
            mapCenterPosition.Z = -(mapInfoHeight - 1) / 2.0f * cellSize;

            normalVectorInfo = new Vector3[mapInfoWidth, mapInfoHeight];
            for (int x = 0; x < mapInfoWidth; x++) {
                for (int y = 0; y < mapInfoHeight; y++) {
                    int nextX = x + 1;
                    int nextY = y + 1;
                    if (nextX == mapInfoWidth) nextX -= 2;
                    if (nextY == mapInfoHeight) nextY -= 2;

                    Vector3 currentPoint = new Vector3(x * cellSize, heightInfo[x, y], y * cellSize);
                    Vector3 nextXPoint = new Vector3(nextX * cellSize, heightInfo[nextX, y], y * cellSize);
                    Vector3 nextYPoint = new Vector3(x * cellSize, heightInfo[x, nextY], nextY * cellSize);

                    Vector3 v1 = currentPoint - nextXPoint;
                    Vector3 v2 = currentPoint - nextYPoint;
                    Vector3 normal = Vector3.Cross(v1, v2);
                    normal.Normalize();
                    if (normal.Y < 0) normal = -normal;
                    normalVectorInfo[x, y] = normal;
                }
            }

            this.cellSize = cellSize;
            cellTexture = Game.Content.Load<Texture2D>(@"Sprites\rocks");

            effect = Game.Services.GetService<BasicEffect>();
            camera = Game.Services.GetService<Camera>();
            SetUpVerticesAndIndices();

            vertexBuffer = new VertexBuffer(GraphicsDevice, vertices[0].GetType(), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);
        }

        private void SetUpVerticesAndIndices() {
            int mapInfoWidth = heightInfo.GetLength(0);
            int mapInfoHeight = heightInfo.GetLength(1);

            // Setup Vertices
            vertices = new VertexPositionNormalTexture[mapInfoWidth * mapInfoHeight];
            for (int x=0; x<mapInfoWidth; x++) {
                for (int y=0; y<mapInfoHeight; y++) {
                    vertices[x + y * mapInfoWidth].Position = new Vector3(x * cellSize, heightInfo[x, y], y * cellSize);
                    vertices[x + y * mapInfoWidth].Normal = normalVectorInfo[x, y];
                    vertices[x + y * mapInfoWidth].TextureCoordinate = new Vector2(x * 1.0f / mapInfoWidth, y * 1.0f / mapInfoHeight);
                }
            }

            // Setup Indices
            vertexIndices = new int[(mapInfoWidth - 1) * (mapInfoHeight - 1) * 6];

            int i = 0;
            for (int y=0; y< mapInfoHeight-1; y++) {
                for (int x=0; x < mapInfoWidth-1; x++) {
                    int lowerLeft = x + y * mapInfoWidth;
                    int lowerRight = (x + 1) + y * mapInfoWidth;
                    int topLeft = x + (y + 1) * mapInfoWidth;
                    int topRight = (x + 1) + (y + 1) * mapInfoWidth;

                    vertexIndices[i++] = topLeft;
                    vertexIndices[i++] = lowerRight;
                    vertexIndices[i++] = lowerLeft;

                    vertexIndices[i++] = topLeft;
                    vertexIndices[i++] = topRight;
                    vertexIndices[i++] = lowerRight;
                }
            }
        }

        public override void Draw(GameTime gameTime) {

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            effect.TextureEnabled = true;
            effect.Texture = cellTexture;
            effect.World = Matrix.Identity;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, vertexIndices, 0, vertexIndices.Length / 3);
            }
        }

        public bool isInMapRange(Vector3 position) {
            Vector3 relativePosition = position - mapCenterPosition;

            return (0 < relativePosition.X && relativePosition.X < mapWidth
                && 0 < relativePosition.Z && relativePosition.Z < mapHeight);
        }

        public void getNormalAndHeight(Vector3 position, out float height, out Vector3 normal) {
            Vector3 relativePosition = position - mapCenterPosition;

            int left, top;
            left = (int)(relativePosition.X / cellSize);
            top = (int)(relativePosition.Z / cellSize);

            float xNormalized = (relativePosition.X % cellSize) / cellSize;
            float zNormalized = (relativePosition.Z % cellSize) / cellSize;

            float topHeight = MathHelper.Lerp(heightInfo[left, top], heightInfo[left + 1, top], xNormalized);
            float bottomHeight = MathHelper.Lerp(heightInfo[left, top + 1], heightInfo[left + 1, top + 1], xNormalized);
            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);

            Vector3 topNormal = Vector3.Lerp(normalVectorInfo[left, top], normalVectorInfo[left + 1, top], xNormalized);
            Vector3 bottomNormal = Vector3.Lerp(normalVectorInfo[left + 1, top + 1], normalVectorInfo[left + 1, top + 1], xNormalized);
            normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
            normal.Normalize();
        }
    }
}
