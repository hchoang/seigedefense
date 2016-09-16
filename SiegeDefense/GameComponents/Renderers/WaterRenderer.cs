using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class WaterRenderer : Renderer {

        private RenderTarget2D refractionRenderTarget;
        private RenderTarget2D reflectionRenderTarget;

        private float waterHeight;
        private VertexBuffer waterVertexBuffer;
        private VertexPositionTexture[] waterVertices;
        private Texture2D waterBumpMap;
        private Vector3 windDirection = new Vector3(1, 0, 0);
        private Matrix reflectionViewMatrix;

        private HeightMap map {
            get { return (HeightMap)baseObject; }
        }

        private Skybox _sky;
        protected Skybox sky {
            get {
                if (_sky == null) {
                    _sky = FindObjects<Skybox>()[0];
                }
                return _sky;
            }
        }

        public WaterRenderer(Vector2 corner1, Vector2 corner2, float waterHeight) {
            waterBumpMap = Game.Content.Load<Texture2D>(@"Terrain\waterbump");
            this.waterHeight = waterHeight;

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            refractionRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, pp.BackBufferFormat, pp.DepthStencilFormat);
            reflectionRenderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, true, pp.BackBufferFormat, pp.DepthStencilFormat);


            waterVertices = new VertexPositionTexture[6];
            waterVertices[0] = new VertexPositionTexture(new Vector3(corner1.X, waterHeight, corner1.Y), new Vector2(0, 1));
            waterVertices[1] = new VertexPositionTexture(new Vector3(corner1.X, waterHeight, corner2.Y), new Vector2(0, 0));
            waterVertices[2] = new VertexPositionTexture(new Vector3(corner2.X, waterHeight, corner2.Y), new Vector2(1, 0));
            

            waterVertices[3] = new VertexPositionTexture(new Vector3(corner1.X, waterHeight, corner1.Y), new Vector2(0, 1));
            waterVertices[4] = new VertexPositionTexture(new Vector3(corner2.X, waterHeight, corner2.Y), new Vector2(1, 0));
            waterVertices[5] = new VertexPositionTexture(new Vector3(corner2.X, waterHeight, corner1.Y), new Vector2(1, 1));

            waterVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionTexture.VertexDeclaration, waterVertices.Length, BufferUsage.WriteOnly);
            waterVertexBuffer.SetData(waterVertices);
        }

        public override void Draw(GameTime gameTime) {
            DrawReflectionMap(gameTime);
            DrawRefractionMap(gameTime);
            DrawWater(gameTime);
            base.Draw(gameTime);
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

            customEffect.CurrentTechnique = customEffect.Techniques["MapRefraction"];
            sky.renderer.DrawReflection(gameTime, reflectionViewMatrix, reflCameraPosition);
            customEffect.Parameters["ClipPlane"].SetValue(new Vector4(Vector3.Up, -waterHeight));
            map.renderer.DrawMap(customEffect, reflectionViewMatrix);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.PresentationParameters.RenderTargetUsage = oldUseage;
        }

        private void DrawRefractionMap(GameTime gameTime) {
            RenderTargetUsage oldUseage = GraphicsDevice.PresentationParameters.RenderTargetUsage;
            GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            GraphicsDevice.SetRenderTarget(refractionRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            customEffect.CurrentTechnique = customEffect.Techniques["MapRefraction"];
            customEffect.Parameters["ClipPlane"].SetValue(new Vector4(Vector3.Down, waterHeight));
            map.renderer.DrawMap(customEffect, camera.ViewMatrix);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.PresentationParameters.RenderTargetUsage = oldUseage;
        }

        private void DrawWater(GameTime gameTime) {
            RasterizerState oldRsState = GraphicsDevice.RasterizerState;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            customEffect.CurrentTechnique = customEffect.Techniques["Water"];
            customEffect.Parameters["World"].SetValue(transformation.WorldMatrix);
            customEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            customEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            customEffect.Parameters["ReflectionView"].SetValue(reflectionViewMatrix);
            customEffect.Parameters["ReflectionMap"].SetValue(reflectionRenderTarget);
            customEffect.Parameters["WaterBumpMap"].SetValue(waterBumpMap);
            customEffect.Parameters["WaveLength"].SetValue(0.3f);
            customEffect.Parameters["WaveHeight"].SetValue(0.01f);
            customEffect.Parameters["RefractionMap"].SetValue(refractionRenderTarget);
            customEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            customEffect.Parameters["Time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            customEffect.Parameters["WindForce"].SetValue(0.02f);
            customEffect.Parameters["WindDirection"].SetValue(windDirection);

            foreach (EffectPass pass in customEffect.CurrentTechnique.Passes) {
                pass.Apply();

                GraphicsDevice.SetVertexBuffer(waterVertexBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, waterVertices.Length / 3);
            }

            GraphicsDevice.RasterizerState = oldRsState;
        }
    }
}
