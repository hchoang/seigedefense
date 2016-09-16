using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public class SkyRenderer : Renderer {

        private TextureCube morningSkytexture;
        private TextureCube afternoonSkyTexture;
        private TextureCube sunsetSkyTexture;
        private TextureCube nightSkyTexture;
        private Vector4 textureWeight = Vector4.Zero;
        private Model skyModel;

        public SkyRenderer() {
            morningSkytexture = Game.Content.Load<TextureCube>(@"Sky\morningSky");
            afternoonSkyTexture = Game.Content.Load<TextureCube>(@"Sky\afternoonSky");
            sunsetSkyTexture = Game.Content.Load<TextureCube>(@"Sky\sunsetSky");
            nightSkyTexture = Game.Content.Load<TextureCube>(@"Sky\nightSky");
            skyModel = Game.Content.Load<Model>(@"Sky\cube");

            customEffect.CurrentTechnique = customEffect.Techniques["DayNightSkybox"];

            skyModel.Meshes[0].MeshParts[0].Effect = customEffect.Clone();
        }
        

        private void CalculateTextureWeight(GameTime gameTime) {
            float time = (float)gameTime.TotalGameTime.TotalSeconds % 24;
            float morning = MathHelper.Clamp(1 - Math.Abs(time - 8) / 4, 0, 1);
            float afternoon = MathHelper.Clamp(1 - Math.Abs(time - 14) / 4, 0, 1);
            float sunset = MathHelper.Clamp(1 - Math.Abs(time - 20) / 4, 0, 1);
            float night = 0;

            if (time < 6) {
                night = 1 - Math.Abs(time - 2) / 4;
            } else if (time > 22) {
                night = (time - 22) / 4;
            }

            float total = morning + afternoon + sunset + night;
            morning /= total;
            afternoon /= total;
            sunset /= total;
            night /= total;
            textureWeight = new Vector4(morning, afternoon, sunset, night);
        }

        public override void Draw(GameTime gameTime) {
            // store graphics device state
            RasterizerState oldRsState = GraphicsDevice.RasterizerState;
            DepthStencilState oldDsState = GraphicsDevice.DepthStencilState;

            DepthStencilState newDsState = new DepthStencilState();
            newDsState.DepthBufferEnable = false;

            RasterizerState newRsState = new RasterizerState();
            newRsState.CullMode = CullMode.None;

            GraphicsDevice.DepthStencilState = newDsState;
            GraphicsDevice.RasterizerState = newRsState;

            Matrix[] modelTransforms = new Matrix[skyModel.Bones.Count];
            skyModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            CalculateTextureWeight(gameTime);

            foreach (ModelMesh mesh in skyModel.Meshes) {
                foreach (Effect effect in mesh.Effects) {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * baseObject.transformation.WorldMatrix;
                    effect.Parameters["World"].SetValue(worldMatrix);
                    effect.Parameters["View"].SetValue(camera.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    effect.Parameters["morningSkyTexture"].SetValue(morningSkytexture);
                    effect.Parameters["afternoonSkyTexture"].SetValue(afternoonSkyTexture);
                    effect.Parameters["sunsetSkyTexture"].SetValue(sunsetSkyTexture);
                    effect.Parameters["nightSkyTexture"].SetValue(nightSkyTexture);
                    effect.Parameters["timeWeight"].SetValue(textureWeight);
                    effect.Parameters["CameraPosition"].SetValue(camera.Position);
                }
                mesh.Draw();
            }

            GraphicsDevice.DepthStencilState = oldDsState;
            GraphicsDevice.RasterizerState = oldRsState;

            base.Draw(gameTime);
        }

        public void DrawReflection(GameTime gameTime, Matrix reflectionViewMatrix, Vector3 reflCamPos) {
            RasterizerState oldRsState = GraphicsDevice.RasterizerState;
            DepthStencilState oldDsState = GraphicsDevice.DepthStencilState;
            DepthStencilState newDsState = new DepthStencilState();
            newDsState.DepthBufferEnable = false;
            RasterizerState newRsState = new RasterizerState();
            newRsState.CullMode = CullMode.None;
            GraphicsDevice.DepthStencilState = newDsState;
            GraphicsDevice.RasterizerState = newRsState;

            Matrix[] modelTransforms = new Matrix[skyModel.Bones.Count];
            skyModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            CalculateTextureWeight(gameTime);

            Vector3 staticCameraPos = Vector3.Zero;
            Vector3 staticCameraTarget = new Vector3(10, 0, 10);
            Vector3 staticCameraUp = Vector3.Up;

            foreach (ModelMesh mesh in skyModel.Meshes) {
                foreach (Effect effect in mesh.Effects) {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * baseObject.transformation.WorldMatrix;

                    effect.Parameters["World"].SetValue(worldMatrix);
                    effect.Parameters["View"].SetValue(reflectionViewMatrix);
                    effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    effect.Parameters["morningSkyTexture"].SetValue(morningSkytexture);
                    effect.Parameters["afternoonSkyTexture"].SetValue(afternoonSkyTexture);
                    effect.Parameters["sunsetSkyTexture"].SetValue(sunsetSkyTexture);
                    effect.Parameters["nightSkyTexture"].SetValue(nightSkyTexture);
                    effect.Parameters["timeWeight"].SetValue(textureWeight);
                    effect.Parameters["CameraPosition"].SetValue(reflCamPos);
                }
                mesh.Draw();
            }

            GraphicsDevice.DepthStencilState = oldDsState;
            GraphicsDevice.RasterizerState = oldRsState;
        }
    }
}
