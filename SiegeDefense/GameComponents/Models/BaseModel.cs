using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Design;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;
using System;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.Models
{
    public class BaseModel : _3DGameObject
    {
        public Model model { get; protected set; }
        private Matrix[] absoluteTranform;
        public Matrix[] relativeTransform;

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
            AddChild(new OrientedCollisionBox(model));
            Position = PositionGenerate();

            absoluteTranform = new Matrix[model.Bones.Count];
            relativeTransform = new Matrix[model.Bones.Count];
            foreach(ModelBone bone in model.Bones)
            {
                relativeTransform[bone.Index] = bone.Transform;
            }
        }

        public BaseModel(Model model, Vector3 Position) : this(model)
        {
            this.Position = Position;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }

        public override void Draw(GameTime gameTime)
        {
            model.CopyBoneTransformsFrom(relativeTransform);
            model.CopyAbsoluteBoneTransformsTo(absoluteTranform);
            foreach (ModelMesh mesh in model.Meshes)
            {   
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.World = absoluteTranform[mesh.ParentBone.Index] * WorldMatrix;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public Vector3 PositionGenerate()
        {
            Random rnd = new Random();
            //Vector3 position =  new Vector3(rnd.Next(0, 500), 0, rnd.Next(0, 500));
            Vector3 position = new Vector3(500, 0, 500);
            float height;
            do
            {
                height = map.GetHeight(position);
            } while (height > 100);
            
            
            position = position + new Vector3(0, height, 0);
            return position;
        }
    }
}
