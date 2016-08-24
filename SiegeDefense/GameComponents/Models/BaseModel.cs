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
        protected BoundingBox bounding;
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
            bounding = CalculateBounding();
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
                BoundingBox box = BoundingBox.CreateFromSphere(mesh.BoundingSphere);
                bounding = BoundingBox.CreateMerged(this.bounding, box);
                
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.World = absoluteTranform[mesh.ParentBone.Index] * WorldMatrix;
                }

                mesh.Draw();
            }
        }

        public BoundingBox CalculateBounding()
        {
            bounding = new BoundingBox();
            Matrix[] transform = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transform);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Vector3 meshBoundingSphereCenter = mesh.BoundingSphere.Center;
                float meshBoundingSphereRadius = mesh.BoundingSphere.Radius;

                Vector3 newCenter = Vector3.Transform(meshBoundingSphereCenter, transform[mesh.ParentBone.Index]);
                Vector3 boneScale = transform[mesh.ParentBone.Index].Scale;
                float maxScale = MathHelper.Max(boneScale.X, boneScale.Y);
                maxScale = MathHelper.Max(maxScale, boneScale.Z);
                float newRadius = meshBoundingSphereRadius * maxScale;

                BoundingBox box = BoundingBox.CreateFromSphere(new BoundingSphere(newCenter, newRadius));
                bounding = BoundingBox.CreateMerged(bounding, box);
            }
            return bounding;
        }

        protected BoundingBox CalculateBounding2() {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart meshPart in mesh.MeshParts) {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float)) {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), WorldMatrix);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
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
