using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Models;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.Physics {
    public class OrientedCollisionBox : _3DGameObject {
        protected static Dictionary<Model, BoundingBox> baseBoundingBoxCaching = new Dictionary<Model, BoundingBox>();

        protected BoundingBox baseBoundingBox;
        protected BasicEffect basicEffect;

        protected Camera _camera;
        protected Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }

        protected BaseModel _baseModel;
        protected BaseModel baseModel {
            get {
                if (_baseModel == null) {
                    _baseModel = FindComponent<BaseModel>();
                }
                return _baseModel;
            }
        }

        public OrientedCollisionBox(Model model) {
            basicEffect = Game.Services.GetService<BasicEffect>();

            if (!baseBoundingBoxCaching.ContainsKey(model)) {
                baseBoundingBox = new BoundingBox();
                Matrix[] transform = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transform);

                foreach (ModelMesh mesh in model.Meshes) {
                    foreach (ModelMeshPart part in mesh.MeshParts) {
                        float[] vbData = new float[part.VertexBuffer.VertexDeclaration.VertexStride * part.VertexBuffer.VertexCount / sizeof(float)];
                        part.VertexBuffer.GetData(vbData);

                        Vector3 min = new Vector3(vbData[0], vbData[1], vbData[2]);
                        Vector3 max = min;
                        for (int i = 0; i < vbData.Length; i += part.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float)) {
                            Vector3 pos = new Vector3(vbData[i], vbData[i + 1], vbData[i + 2]);

                            min = Vector3.Min(min, pos);
                            max = Vector3.Max(max, pos);
                        }

                        /*VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[part.VertexBuffer.VertexCount];
                        part.VertexBuffer.GetData(vertices);
                        Vector3 min = vertices[0].Position;
                        Vector3 max = vertices[0].Position;

                        for (int i = 1; i < vertices.Length; i++) {
                            min = Vector3.Min(min, vertices[i].Position);
                            max = Vector3.Max(max, vertices[i].Position);
                        }*/

                        min = Vector3.Transform(min, transform[mesh.ParentBone.Index]);
                        max = Vector3.Transform(max, transform[mesh.ParentBone.Index]);

                        baseBoundingBox.Min = Vector3.Min(baseBoundingBox.Min, min);
                        baseBoundingBox.Max = Vector3.Max(baseBoundingBox.Max, max);
                    }
                }
            }
            else {
                baseBoundingBox = baseBoundingBoxCaching[model];
            }
        }

        public override void Draw(GameTime gameTime) {
            Vector3[] boundingCorners = baseBoundingBox.GetCorners();
            Matrix refWorldMatrix = baseModel.WorldMatrix;
            Vector3.Transform(boundingCorners, ref refWorldMatrix, boundingCorners);

            VertexPositionColor[] vertices = new VertexPositionColor[8];

            for (int i = 0; i < 8; i++) {
                vertices[i].Position = boundingCorners[i];
                vertices[i].Color = Color.Blue;
            }

            int[] indices = new int[24];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 1;
            indices[3] = 2;
            indices[4] = 2;
            indices[5] = 3;
            indices[6] = 3;
            indices[7] = 0;

            indices[8] = 4;
            indices[9] = 5;
            indices[10] = 5;
            indices[11] = 6;
            indices[12] = 6;
            indices[13] = 7;
            indices[14] = 7;
            indices[15] = 4;

            indices[16] = 0;
            indices[17] = 4;
            indices[18] = 1;
            indices[19] = 5;
            indices[20] = 2;
            indices[21] = 6;
            indices[22] = 3;
            indices[23] = 7;

            if (basicEffect == null)
                basicEffect = (BasicEffect)Game.Services.GetService<BasicEffect>().Clone();

            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.FogEnabled = false;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 8, indices, 0, 12);
            }

            base.Draw(gameTime);
        }
    }
}
