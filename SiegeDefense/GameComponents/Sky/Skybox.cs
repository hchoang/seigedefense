using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using System;

namespace SiegeDefense.GameComponents.Sky {
    public class Skybox : _3DGameObject {
        private TextureCube morningSkytexture;
        private TextureCube afternoonSkyTexture;
        private TextureCube sunsetSkyTexture;
        private TextureCube nightSkyTexture;
        private Model skyModel;

        private Effect advancedEffect;
        private BasicEffect basicEffect;
        private Camera camera;

        public Skybox() {
            morningSkytexture = Game.Content.Load<TextureCube>(@"Sky\morningSky");
            afternoonSkyTexture = Game.Content.Load<TextureCube>(@"Sky\afternoonSky");
            sunsetSkyTexture = Game.Content.Load<TextureCube>(@"Sky\sunsetSky");
            nightSkyTexture = Game.Content.Load<TextureCube>(@"Sky\nightSky");
            skyModel = Game.Content.Load<Model>(@"Sky\cube");

            advancedEffect = Game.Services.GetService<Effect>();
            basicEffect = Game.Services.GetService<BasicEffect>();
            ScaleMatrix = Matrix.CreateScale(10);
        }

        public override void GetDependentComponents() {
            camera = (Camera)FindObjectsByTag("Camera")[0];
        }

        public override void Update(GameTime gameTime) {
            TranslationMatrix = Matrix.CreateTranslation(camera.Position);
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

            float time = (float)gameTime.TotalGameTime.TotalSeconds % 24;
            float morning = MathHelper.Clamp(1 - Math.Abs(time - 8) / 4, 0, 1);
            float afternoon = MathHelper.Clamp(1 - Math.Abs(time - 14) / 4, 0, 1);
            float sunset = MathHelper.Clamp(1 - Math.Abs(time - 20) / 4, 0, 1);
            float night = 0;

            if (time < 6) {
                night = 1 - Math.Abs(time - 2) / 4;
            }
            else if (time > 22) {
                night = (time - 22) / 4;
            }

            float total = morning + afternoon + sunset + night;
            morning /= total;
            afternoon /= total;
            sunset /= total;
            night /= total;
            Vector4 timeWeight = new Vector4(morning, afternoon, sunset, night);

            advancedEffect.CurrentTechnique = advancedEffect.Techniques["DayNightSkybox"];
            foreach (EffectPass pass in advancedEffect.CurrentTechnique.Passes) {
                pass.Apply();
                foreach (ModelMesh mesh in skyModel.Meshes) {
                    foreach (ModelMeshPart part in mesh.MeshParts) {
                        part.Effect = advancedEffect;
                        part.Effect.Parameters["World"].SetValue(WorldMatrix);
                        part.Effect.Parameters["morningSkyTexture"].SetValue(morningSkytexture);
                        part.Effect.Parameters["afternoonSkyTexture"].SetValue(afternoonSkyTexture);
                        part.Effect.Parameters["sunsetSkyTexture"].SetValue(sunsetSkyTexture);
                        part.Effect.Parameters["nightSkyTexture"].SetValue(nightSkyTexture);
                        part.Effect.Parameters["timeWeight"].SetValue(timeWeight);
                        part.Effect.Parameters["CameraPosition"].SetValue(camera.Position);
                    }
                    mesh.Draw();
                }
            }

            GraphicsDevice.DepthStencilState = oldDsState;
            GraphicsDevice.RasterizerState = oldRsState;
        }
    }
}
