using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Sky;
using System;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.Maps {
    public partial class HeightMap {
        private Texture2D sandTexture;
        private Texture2D grassTexture;
        private Texture2D rockTexture;
        private Texture2D snowTexture;
        
        private Effect multiTexturedEffect;

        // Map attributes
        public string test { get; set; }
        private int mapInfoWidth;
        private int mapInfoHeight;
        private float minMapHeight = 0;
        private float maxMapHeight = 255;

        // Map configs - will be configued via constructor/setter
        private float mapCellSize = 10.0f;
        private float mapDeltaHeight = 100.0f;

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

        // Water
        private Camera _camera;
        private Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }
        private Skybox _sky;
        private Skybox sky {
            get {
                if (_sky == null) {
                    _sky = FindObjects<Skybox>()[0];
                }
                return _sky;
            }
        }
        private float waterHeight = 0.2f;
        private Effect reflectionTechnique;
        private Effect refractionTechnique;
        private Effect waterTechnique;
        private RenderTarget2D refractionRenderTarget;
        private RenderTarget2D reflectionRenderTarget;
        private Matrix reflectionViewMatrix;
        private VertexBuffer waterVertexBuffer;
        private VertexPositionTexture[] waterVertices;
        private Texture2D waterBumpMap;
        private Vector3 windDirection = new Vector3(1, 0, 0);

        public HeightMap(LevelDescription description) {
            this.mapCellSize = description.MapCellSize;
            this.mapDeltaHeight = description.MapDeltaHeight;

            InitMap();

            foreach (Vector3 spawnPoint in description.SpawnPoints) {
                float height = GetHeight(spawnPoint * mapCellSize);
                this.SpawnPoints.Add(new Vector3(spawnPoint.X * mapCellSize, height, spawnPoint.Z * mapCellSize));
            }

            Vector3 playerPos = description.PlayerStartPoint * mapCellSize;
            playerPos.Y = GetHeight(PlayerStartPosition);
            this.PlayerStartPosition = playerPos;

            Vector3 HQPos = description.HeadquarterPosition * mapCellSize;
            HQPos.Y = GetHeight(HQPos);
            this.HeadquarterPosition = HQPos;
        }

        private void InitMap() {
            sandTexture = Game.Content.Load<Texture2D>(@"Terrain\sand");
            grassTexture = Game.Content.Load<Texture2D>(@"Terrain\grass");
            rockTexture = Game.Content.Load<Texture2D>(@"Terrain\rock");
            snowTexture = Game.Content.Load<Texture2D>(@"Terrain\snow");
            waterBumpMap = Game.Content.Load<Texture2D>(@"Terrain\waterbump");

            multiTexturedEffect = Game.Services.GetService<Effect>().Clone();
            reflectionTechnique = Game.Services.GetService<Effect>().Clone();
            refractionTechnique = Game.Services.GetService<Effect>().Clone();
            waterTechnique = Game.Services.GetService<Effect>().Clone();

            multiTexturedEffect.CurrentTechnique = multiTexturedEffect.Techniques["MultiTextured"];
            waterTechnique.CurrentTechnique = waterTechnique.Techniques["Water"];
            refractionTechnique.CurrentTechnique = refractionTechnique.Techniques["MapRefraction"];

            multiTexturedEffect.Parameters["EnableLighting"].SetValue(true);
            multiTexturedEffect.Parameters["Ambient"].SetValue(0.4f);
            multiTexturedEffect.Parameters["LightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            ReadTerrainFromTexture();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            refractionRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, pp.BackBufferFormat, pp.DepthStencilFormat);
            reflectionRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, pp.BackBufferFormat, pp.DepthStencilFormat);

            waterHeight = waterHeight * mapDeltaHeight;
            SetupWater();

            GenerateMapNode();
        }

        public override void Draw(GameTime gameTime) {
            DrawRefractionMap(gameTime);
            DrawReflectionMap(gameTime);

            DrawMap(multiTexturedEffect, camera.ViewMatrix);

            DrawWater(gameTime);
        }

        private void DrawWater(GameTime gameTime) {
            RasterizerState oldRsState = GraphicsDevice.RasterizerState;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            Vector3 staticCameraPos = Vector3.Zero;
            Vector3 staticCameraTarget = new Vector3(10, 0, 10);
            Vector3 staticCameraUp = Vector3.Up;

            waterTechnique.Parameters["World"].SetValue(WorldMatrix);
            waterTechnique.Parameters["View"].SetValue(camera.ViewMatrix);
            waterTechnique.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            waterTechnique.Parameters["ReflectionView"].SetValue(reflectionViewMatrix);
            waterTechnique.Parameters["ReflectionMap"].SetValue(reflectionRenderTarget);
            waterTechnique.Parameters["WaterBumpMap"].SetValue(waterBumpMap);
            waterTechnique.Parameters["WaveLength"].SetValue(0.3f);
            waterTechnique.Parameters["WaveHeight"].SetValue(0.01f);
            waterTechnique.Parameters["RefractionMap"].SetValue(refractionRenderTarget);
            waterTechnique.Parameters["CameraPosition"].SetValue(camera.Position);
            waterTechnique.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            waterTechnique.Parameters["WindForce"].SetValue(0.02f);
            waterTechnique.Parameters["WindDirection"].SetValue(windDirection);

            foreach (EffectPass pass in waterTechnique.CurrentTechnique.Passes) {
                pass.Apply();

                GraphicsDevice.SetVertexBuffer(waterVertexBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, waterVertices.Length / 3);
            }

            GraphicsDevice.RasterizerState = oldRsState;
        }

        private void DrawReflectionMap(GameTime gameTime) {
            // calculate reflection matrix
            Vector3 staticCameraPos = Vector3.Zero;
            Vector3 staticCameraTarget = new Vector3(10, 0, 10);
            Vector3 staticCameraUp = Vector3.Up;

            Vector3 reflCameraPosition = camera.Position;
            reflCameraPosition.Y = -camera.Position.Y + waterHeight * 2;
            Vector3 reflTargetPos = camera.Target;
            reflTargetPos.Y = -camera.Target.Y + waterHeight * 2;
            Vector3 cameraRight = -camera.Left;
            Vector3 invUpVector = Vector3.Cross(cameraRight, reflTargetPos - reflCameraPosition);
            //Vector3 invUpVector = Vector3.Up;
            
            reflectionViewMatrix = Matrix.CreateLookAt(reflCameraPosition, reflTargetPos, invUpVector);

            // draw reflection map
            RenderTargetUsage oldUseage = GraphicsDevice.PresentationParameters.RenderTargetUsage;
            GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            GraphicsDevice.SetRenderTarget(reflectionRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            sky.DrawReflection(gameTime, reflectionViewMatrix, reflCameraPosition);
            refractionTechnique.Parameters["ClipPlane"].SetValue(new Vector4(Vector3.Up, -waterHeight));
            DrawMap(refractionTechnique, reflectionViewMatrix);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.PresentationParameters.RenderTargetUsage = oldUseage;
        }

        private void DrawRefractionMap(GameTime gameTime) {
            RenderTargetUsage oldUseage = GraphicsDevice.PresentationParameters.RenderTargetUsage;
            GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            GraphicsDevice.SetRenderTarget(refractionRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            refractionTechnique.Parameters["ClipPlane"].SetValue(new Vector4(Vector3.Down, waterHeight));
            DrawMap(refractionTechnique, camera.ViewMatrix);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.PresentationParameters.RenderTargetUsage = oldUseage;
        }

        private void DrawMap(Effect effect, Matrix viewMatrix) {
            effect.Parameters["World"].SetValue(WorldMatrix);
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

        private void SetupWater() {
            waterVertices = new VertexPositionTexture[6];

            waterVertices[0] = new VertexPositionTexture(new Vector3(0, waterHeight, 0), new Vector2(0, 1));
            waterVertices[2] = new VertexPositionTexture(new Vector3(mapInfoWidth * mapCellSize, waterHeight, mapInfoHeight * mapCellSize), new Vector2(1, 0));
            waterVertices[1] = new VertexPositionTexture(new Vector3(0, waterHeight, mapInfoHeight * mapCellSize), new Vector2(0, 0));

            waterVertices[3] = new VertexPositionTexture(new Vector3(0, waterHeight, 0), new Vector2(0, 1));
            waterVertices[5] = new VertexPositionTexture(new Vector3(mapInfoWidth * mapCellSize, waterHeight, 0), new Vector2(1, 1));
            waterVertices[4] = new VertexPositionTexture(new Vector3(mapInfoWidth * mapCellSize, waterHeight, mapInfoHeight * mapCellSize), new Vector2(1, 0));

            waterVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, waterVertices.Length, BufferUsage.WriteOnly);
            waterVertexBuffer.SetData(waterVertices);
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

                    //if (heightInfo[x, y] < minMapHeight) minMapHeight = heightInfo[x, y];
                    //if (heightInfo[x, y] > maxMapHeight) maxMapHeight = heightInfo[x, y];
                }
            }

            // normalize height data to [0-mapDeltaHeight]
            //if (maxMapHeight == minMapHeight) maxMapHeight = minMapHeight + 1;
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
