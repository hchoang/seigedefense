using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;
using System;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.SoundBank;
using System.ComponentModel;

namespace SiegeDefense.GameComponents.Models
{
    public enum ModelType {
        [Description(@"Models\tank")]
        TANK1,
        [Description(@"Models\bullet")]
        BULLET1
    }

    public class BaseModel : _3DGameObject
    {
        public Model model { get; protected set; }
        protected Matrix[] absoluteTranform;
        protected Matrix[] relativeTransform;
        
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

        protected SoundBankManager soundManager;
        
        public OrientedCollisionBox collisionBox { get; set; }
        public GamePhysics physics { get; set; }

        public BaseModel(ModelType modelType)
        {
            model = Game.Content.Load<Model>(modelType.ToDescription());

            collisionBox = new OrientedCollisionBox(this);
            physics = new GamePhysics(this);
            AddChild(collisionBox);
            AddChild(physics);

            soundManager = Game.Services.GetService<SoundBankManager>();

            absoluteTranform = new Matrix[model.Bones.Count];
            relativeTransform = new Matrix[model.Bones.Count];
            foreach(ModelBone bone in model.Bones)
            {
                relativeTransform[bone.Index] = bone.Transform;
            }
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

        public float distanceCaculator(BaseModel model)
        {
            return Vector3.Distance(this.Position, model.Position);
        }
    }
}
