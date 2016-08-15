using System;
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.Physics {
    public class CollisionBox : CollisionBoundary {
        public Vector3 Size { get; set; }

        public CollisionBox(Vector3 Center, Vector3 Size) {

        }

        public override bool IsInside(Vector3 point) {

            if (Math.Abs(point.X - Position.X) > Size.X / 2) return false;

            if (Math.Abs(point.Y - Position.Y) > Size.Y / 2) return false;

            if (Math.Abs(point.Z - Position.Z) > Size.Z / 2) return false;

            return true;
        }

        public override float GroundDistance(Map map) {
            return map.GetHeight(Position) - Size.Y;
        }
    }
}
