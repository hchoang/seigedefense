using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SiegeDefense {
    public class Collider : GameObjectComponent {
        protected static Dictionary<int, BoundingBox> boundingBoxCaching = new Dictionary<int, BoundingBox>();
        protected static Dictionary<int, BoundingSphere> boundingSphereCaching = new Dictionary<int, BoundingSphere>();

        public BoundingBox baseBoundingBox { get; set; }
        public BoundingSphere baseBoundingSphere { get; set; }
        
        public Collider(BoundingBox baseBoundingBox) {
            this.baseBoundingBox = baseBoundingBox;
            baseBoundingSphere = BoundingSphere.CreateFromBoundingBox(baseBoundingBox);
        }
        
        public Collider(Model model) {
            if (!boundingBoxCaching.ContainsKey(model.GetHashCode())) {
                baseBoundingBox = new BoundingBox();
                baseBoundingSphere = new BoundingSphere();
                Matrix[] transform = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transform);
                foreach (ModelMesh mesh in model.Meshes) {

                    BoundingSphere additionalSphere = mesh.BoundingSphere;
                    additionalSphere.Center = Vector3.Transform(additionalSphere.Center, transform[mesh.ParentBone.Index]);
                    Vector3 scale = transform[mesh.ParentBone.Index].Scale;
                    float maxScale = MathHelper.Max(scale.X, scale.Y);
                    maxScale = MathHelper.Max(maxScale, scale.Z);
                    additionalSphere.Radius *= maxScale;

                    baseBoundingSphere = BoundingSphere.CreateMerged(baseBoundingSphere, additionalSphere);

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

                        min = Vector3.Transform(min, transform[mesh.ParentBone.Index]);
                        max = Vector3.Transform(max, transform[mesh.ParentBone.Index]);

                        baseBoundingBox = new BoundingBox(Vector3.Min(baseBoundingBox.Min, min), Vector3.Max(baseBoundingBox.Max, max));
                    }
                }

                boundingBoxCaching.Add(model.GetHashCode(), baseBoundingBox);
                boundingSphereCaching.Add(model.GetHashCode(), baseBoundingSphere);
            }
            else {
                baseBoundingBox = boundingBoxCaching[model.GetHashCode()];
                baseBoundingSphere = boundingSphereCaching[model.GetHashCode()];
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

        public bool SphereIntersect(Collider other) {
            BoundingSphere bounding1 = baseBoundingSphere;
            bounding1.Center = baseObject.transformation.Position;

            BoundingSphere bounding2 = other.baseBoundingSphere;
            bounding2.Center = other.baseObject.transformation.Position;

            return bounding1.Intersects(bounding2);
        }

        private bool Intersect(Vector3[] boundingCorners1, Vector3[] boundingCorners2) {
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

        public bool Intersect(BoundingBox other) {
            Vector3[] boundingCorners1 = baseBoundingBox.GetCorners();
            Matrix refWorldMatrix1 = baseObject.transformation.WorldMatrix;
            Vector3.Transform(boundingCorners1, ref refWorldMatrix1, boundingCorners1);

            Vector3[] boundingCorners2 = other.GetCorners();

            return Intersect(boundingCorners1, boundingCorners2);
        }

        public bool Intersect(Collider other) {
            
            Vector3[] boundingCorners1 = baseBoundingBox.GetCorners();
            Matrix refWorldMatrix1 = baseObject.transformation.WorldMatrix;
            Vector3.Transform(boundingCorners1, ref refWorldMatrix1, boundingCorners1);

            Vector3[] boundingCorners2 = other.baseBoundingBox.GetCorners();
            Matrix refWorldMatrix2 = other.baseObject.transformation.WorldMatrix;
            Vector3.Transform(boundingCorners2, ref refWorldMatrix2, boundingCorners2);

            Vector3[] normal1 = { boundingCorners1[0] - boundingCorners1[1], boundingCorners1[1] - boundingCorners1[2], boundingCorners1[0] - boundingCorners1[4] };
            Vector3[] normal2 = { boundingCorners2[0] - boundingCorners2[1], boundingCorners2[1] - boundingCorners2[2], boundingCorners2[0] - boundingCorners2[4] };

            return Intersect(boundingCorners1, boundingCorners2);
        }
    }
}
