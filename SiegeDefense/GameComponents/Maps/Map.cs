using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.Maps {
    public abstract class Map : _3DGameObject {
        public List<Vector3> SpawnPoints { get; set; } = new List<Vector3>();
        public Vector3 PlayerStartPosition { get; set; }
        public Vector3 HeadquarterPosition { get; set; }
        public abstract float GetHeight(Vector3 position);
        public abstract Vector3 GetNormal(Vector3 position);
        public abstract bool IsInsideMap(Vector3 position);
        public abstract bool IsAccessibleByFoot(Vector3 position);
        public Map() {
            Tag = "Map";
        }
    }
}
