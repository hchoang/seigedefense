using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Design;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;

namespace SiegeDefense.GameComponents.Models
{
    class BaseModel : _3DGameObject
    {
        public Model model { get; protected set; }
        private Matrix scaleMatrix;
        private Vector3 position;
        private BoundingBox bouding;

        public BaseModel(Model model)
        {
            this.model = model;
            scaleMatrix = Matrix.CreateScale(0.5f);
            bouding = this.CalculateBouding();
        }

        public virtual void Update()
        {


        }

        public virtual void Draw(FPSCamera camera)
        {
            Matrix[] transform = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transform);

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingBox box = BoundingBox.CreateFromSphere(mesh.BoundingSphere);
                bouding = BoundingBox.CreateMerged(this.bouding, box);
                
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.World = transform[mesh.ParentBone.Index] * scaleMatrix * WorldMatrix *  Matrix.CreateTranslation(position);
                }

                mesh.Draw();
            }
        }

        public BoundingBox CalculateBouding()
        {
            bouding = new BoundingBox();
            Matrix[] transform = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transform);

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingBox box = BoundingBox.CreateFromSphere(mesh.BoundingSphere);
                bouding = BoundingBox.CreateMerged(this.bouding, box);
            }
            return bouding;
        }
    }
}
