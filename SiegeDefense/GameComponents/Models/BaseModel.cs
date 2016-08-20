using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Design;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;
using System;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.Models
{
    class BaseModel : _3DGameObject
    {
        public Model model { get; protected set; }
        protected BoundingBox bouding;

        private Camera _camera;
        protected Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }
        private Map _map;
        protected Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public BaseModel(Model model)
        {
            this.model = model;
            ScaleMatrix = Matrix.CreateScale(5);
            bouding = CalculateBouding();
            Position = PositionGenerate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
                    effect.World = transform[mesh.ParentBone.Index] * WorldMatrix;
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
            float height = map.GetHeight(position);
            position = position + new Vector3(0, height, 0);
            return position;
        }
    }
}
