using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace SiegeDefense {
    public enum ModelType {
        [Description(@"Models\tank")]
        TANK1,
        [Description(@"Models\bullet")]
        BULLET1
    }

    public class ModelRenderer : Renderer {
        protected Model model;
        protected Matrix[] absoluteTranform;
        protected Matrix[] relativeTransform;

        public ModelRenderer(Model model) {
            this.model = model;

            absoluteTranform = new Matrix[model.Bones.Count];
            relativeTransform = new Matrix[model.Bones.Count];
            for (int i=0; i<model.Bones.Count; i++) {
                relativeTransform[i] = model.Bones[i].Transform;
            }
        }
        
        public override void Draw(GameTime gameTime) {

            model.CopyBoneTransformsFrom(relativeTransform);
            model.CopyAbsoluteBoneTransformsTo(absoluteTranform);
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.World = absoluteTranform[mesh.ParentBone.Index] * baseObject.transformation.WorldMatrix;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
