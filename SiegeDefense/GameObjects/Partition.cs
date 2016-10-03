using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class Partition : _3DGameObject {
        public List<_3DGameObject> managedObjects { get; set; } = new List<_3DGameObject>();
        public static Partition RootPartition { get; protected set; }
        public static Partition[,,] PartitionList;
        private static int X = 20;
        private static int Y = 4;
        private static int Z = 20;
        
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public new WireFrameBoxRenderer renderer { get; set; }

        public BoundingBox boundingBox { get; set; }

        public Partition(BoundingBox boundingBox) {
            this.boundingBox = boundingBox;

            renderer = new WireFrameBoxRenderer(boundingBox.GetCorners(), Color.Blue);
            AddComponent(renderer);
        }

        public Partition(Map map) {
            RootPartition = this;

            this.boundingBox = map.GetBoundingBox();

            Vector3 boundingBoxSize = boundingBox.Max - boundingBox.Min;
            Vector3 unitBoxSize = new Vector3(boundingBoxSize.X / X, boundingBoxSize.Y / Y, boundingBoxSize.Z / Z);

            PartitionList = new Partition[X, Y, Z];
            for (int x=0; x<X; x++) {
                for (int y=0; y<Y; y++) {
                    for (int z=0; z<Z; z++) {
                        Vector3 min = boundingBox.Min + unitBoxSize * new Vector3(x, y, z);
                        Partition newPartition = new Partition(new BoundingBox(min, min + unitBoxSize));
                        newPartition.x = x;
                        newPartition.y = y;
                        newPartition.z = z;
                        PartitionList[x, y, z] = newPartition;
                    }
                }
            }
        }
        
        public void RemoveObject(_3DGameObject gameObject) {
            this.managedObjects.Remove(gameObject);
            Game.Components.Remove(gameObject);
            gameObject.partition = null;
        }

        public void AddObject(_3DGameObject gameObject) {
            if (this == RootPartition) {
                
                Partition partition = FindPartition(gameObject.transformation.Position);
                partition.AddObject(gameObject);
                return;
            }

            this.managedObjects.Add(gameObject);
            Game.Components.Add(gameObject);
            gameObject.partition = this;
        }

        public Partition FindPartition(Vector3 position) {

            Vector3 relativePosition = position - RootPartition.boundingBox.Min;
            Vector3 rootPartitionSize = RootPartition.boundingBox.Max - RootPartition.boundingBox.Min;

            int x = (int)(relativePosition.X * X / rootPartitionSize.X);
            int y = (int)(relativePosition.Y * Y / rootPartitionSize.Y);
            int z = (int)(relativePosition.Z * Z / rootPartitionSize.Z);

            if (x < 0 || x >= X) return null;
            if (y < 0 || y >= Y) return null;
            if (z < 0 || z >= Z) return null;

            return PartitionList[x, y, z];
        }

        public static List<GameObject> GetObjectsForCollisionDetection(_3DGameObject gameObject) {

            List<GameObject> returnValue = new List<GameObject>();

            int pX = gameObject.partition.x;
            int pY = gameObject.partition.y;
            int pZ = gameObject.partition.z;

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    for (int z = -1; z <= 1; z++) {

                        int fX = pX + x;
                        int fY = pY + y;
                        int fZ = pZ + z;

                        if (fX < 0 || fX >= X) continue;
                        if (fY < 0 || fY >= Y) continue;
                        if (fZ < 0 || fZ >= Z) continue;

                        PartitionList[fX, fY, fZ].renderer.SetColor(Color.Yellow);

                        returnValue.AddRange(PartitionList[fX, fY, fZ].managedObjects);

                    }
                }
            }

            return returnValue;
        }

        public override void Draw(GameTime gameTime) {
            if (this == RootPartition) {
                foreach (Partition partition in PartitionList) {
                    partition.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {

            foreach (Partition partition in PartitionList) {

                partition.renderer.SetColor(Color.Blue);

                List<_3DGameObject> reallocateObjects = new List<_3DGameObject>();

                foreach (_3DGameObject managedObject in partition.managedObjects) {
                    if (partition.boundingBox.Contains(managedObject.transformation.Position) == ContainmentType.Disjoint) {
                        reallocateObjects.Add(managedObject);
                    }
                }

                foreach (_3DGameObject reallocateObject in reallocateObjects) {
                    partition.RemoveObject(reallocateObject);
                    Partition newPartition = FindPartition(reallocateObject.transformation.Position);
                    if (newPartition == null) {
                        newPartition = partition;
                    }
                    newPartition.AddObject(reallocateObject);
                }
            }

            base.Update(gameTime);
        }
    }
}
