using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Models;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.Physics {
    public class OrientedCollisionBox : _3DGameObject {
        protected static Dictionary<int, BoundingBox> baseBoundingBoxCaching = new Dictionary<int, BoundingBox>();

        public BoundingBox baseBoundingBox;
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

        protected BaseModel baseModel;

        public OrientedCollisionBox(BaseModel baseModel) {
            this.baseModel = baseModel;
            basicEffect = Game.Services.GetService<BasicEffect>();

            Model model = baseModel.model;
            if (!baseBoundingBoxCaching.ContainsKey(model.GetHashCode())) {
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

                baseBoundingBoxCaching.Add(model.GetHashCode(), baseBoundingBox);
            }
            else {
                baseBoundingBox = baseBoundingBoxCaching[model.GetHashCode()];
            }
        }
        
        private void SATTest(Vector3 axis, Vector3[] corners, out float min, out float max) {
            min = float.MaxValue;
            max = float.MinValue;

            foreach (Vector3 corner in corners) {
                float dotVal = Vector3.Dot(axis, corner);
                if (dotVal < min) min = dotVal;
                if (dotVal > max) max = dotVal;
            }
        }

        private bool overlaps(float min1, float max1, float min2, float max2) {
            return isBetweenOrdered(min2, min1, max1) || isBetweenOrdered(min1, min2, max2);
        }

        private bool isBetweenOrdered(float val, float lowerBound, float upperBound) {
            return lowerBound <= val && val <= upperBound;
        }

        public bool Instersect(OrientedCollisionBox other) {

            Vector3[] boundingCorners1 = baseBoundingBox.GetCorners();
            Matrix refWorldMatrix1 = baseModel.WorldMatrix;
            Vector3.Transform(boundingCorners1, ref refWorldMatrix1, boundingCorners1);

            Vector3[] boundingCorners2 = other.baseBoundingBox.GetCorners();
            Matrix refWorldMatrix2 = other.baseModel.WorldMatrix;
            Vector3.Transform(boundingCorners2, ref refWorldMatrix2, boundingCorners2);

            Vector3[] normal1 = { boundingCorners1[0] - boundingCorners1[1], boundingCorners1[1] - boundingCorners1[2], boundingCorners1[0] - boundingCorners1[4] };
            Vector3[] normal2 = { boundingCorners2[0] - boundingCorners2[1], boundingCorners2[1] - boundingCorners2[2], boundingCorners2[0] - boundingCorners2[4] };

            foreach (Vector3 normal in normal1) {
                float min1, min2, max1, max2;
                SATTest(normal, boundingCorners1, out min1, out max1);
                SATTest(normal, boundingCorners2, out min2, out max2);

                if (!overlaps(min1, max1, min2, max2)) {
                    return false;
                }
            }

            foreach (Vector3 normal in normal2) {
                float min1, min2, max1, max2;
                SATTest(normal, boundingCorners1, out min1, out max1);
                SATTest(normal, boundingCorners2, out min2, out max2);

                if (!overlaps(min1, max1, min2, max2)) {
                    return false;
                }
            }

            return true;
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
