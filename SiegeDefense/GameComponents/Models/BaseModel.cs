using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;
using System;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.Models
{
    public class BaseModel : _3DGameObject
    {
        public Model model { get; protected set; }
        protected Matrix[] absoluteTranform;
        protected Matrix[] relativeTransform;
        public float axisRightFixAngle = 0;
        public float axisUpFixAngle = 0;
        public float axisForwardFixAngle = 0;
        protected Matrix axisRightFixMatrix = Matrix.Identity;
        protected Matrix axisUpFixMatrix = Matrix.Identity;
        protected Matrix axisForwardFixMatrix = Matrix.Identity;
        
        private ModelManager _modelManager;
        protected ModelManager modelManager {
            get {
                if (_modelManager == null) {
                    _modelManager = FindObjects<ModelManager>()[0];
                }
                return _modelManager;
            }
        }

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

        public OrientedCollisionBox collisionBox { get; set; }

        public BaseModel(Model model)
        {
            this.model = model;
            collisionBox = new OrientedCollisionBox(this);
            //AddChild(collisionBox);
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
            axisUpFixMatrix = Matrix.CreateFromAxisAngle(Up, axisUpFixAngle);
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
            Vector3 position = new Vector3(500, 0, 200);
            float height;
            do
            {
                height = map.GetHeight(position);
            } while (height > 100);
            
            
            position = position + new Vector3(0, height, 0);
            return position;
        }

        public float distanceCaculator(BaseModel model)
        {
            return Vector3.Distance(this.Position, model.Position);
        }
    }
}
