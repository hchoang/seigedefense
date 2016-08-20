using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Design;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;
using System;

namespace SiegeDefense.GameComponents.Models
{
    class BaseModel : _3DGameObject
    {
        public Model model { get; protected set; }
        private Matrix scaleMatrix;
        private Vector3 position;
        private BoundingBox bouding;
        private FPSCamera camera;

        public BaseModel(Model model, FPSCamera camera)
        {
            this.model = model;
            this.camera = camera;
            scaleMatrix = Matrix.CreateScale(5);
            bouding = this.CalculateBouding();
            position = this.PositionGenerate();
        }

        public virtual void Update()
        {


        }

        public virtual void Draw()
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

        public Vector3 PositionGenerate()
        {
            Random rnd = new Random();
            Vector3 position =  new Vector3(rnd.Next(0, 500), 0, rnd.Next(0, 500));
            float height = camera.map.GetHeight(position);
            position = position + new Vector3(0, height, 0);
            return position;
        }
    }
}
