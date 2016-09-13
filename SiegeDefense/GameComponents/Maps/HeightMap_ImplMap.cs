using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.Maps {
    public partial class HeightMap : Map{

        public override bool IsAccessibleByFoot(Vector3 position) {

            if (!IsInsideMap(position)) return false;

            Vector3 newPosition = position;
            newPosition.Y = GetHeight(newPosition);
            if (newPosition.Y <= waterHeight + 1) return false;
            if (newPosition.Y > 0.5f * mapDeltaHeight) return false;

            // check map slope
            Vector3 mapNormal = GetNormal(newPosition);
            float angle = MathHelper.Clamp(Vector3.Dot(mapNormal, Vector3.Up) / (mapNormal.Length()), -1, 1);
            angle = (float)Math.Acos(angle);
            float angleInDegree = angle * 180 / MathHelper.Pi;

            if (Math.Abs(angleInDegree) > 40) return false;

            return true;
        }

        public override bool IsInsideMap(Vector3 position) {
            Vector3 firstVertexPosition = vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int gridMapPositionX = (int)(relativePosition.X / mapCellSize);
            int gridMapPositionY = (int)(relativePosition.Z / mapCellSize);

            if (gridMapPositionX < 2 || gridMapPositionX > mapInfoWidth - 3) return false;
            if (gridMapPositionY < 2 || gridMapPositionY > mapInfoHeight - 3) return false;

            return true;
        }

        public override float GetHeight(Vector3 position) {
            Vector3 firstVertexPosition = vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int gridMapPositionX = (int)(relativePosition.X / mapCellSize);
            int gridMapPositionY = (int)(relativePosition.Z / mapCellSize);
            int gridMapPositionNextX = gridMapPositionX + 1;
            int gridMapPositionNextY = gridMapPositionY + 1;

            if (gridMapPositionNextX == mapInfoWidth)
                gridMapPositionNextX -= 2;
            if (gridMapPositionNextY == mapInfoHeight)
                gridMapPositionNextY -= 2;

            float cellPositionX = relativePosition.X % mapCellSize / mapCellSize;
            float cellPositionY = relativePosition.Z % mapCellSize / mapCellSize;

            float h1 = vertices[gridMapPositionX + gridMapPositionY * mapInfoWidth].Position.Y;
            float h2 = vertices[gridMapPositionNextX + gridMapPositionY * mapInfoWidth].Position.Y;
            float h3 = vertices[gridMapPositionX + gridMapPositionNextY * mapInfoWidth].Position.Y;
            float h4 = vertices[gridMapPositionNextX + gridMapPositionNextY * mapInfoWidth].Position.Y;

            float h12 = MathHelper.Lerp(h1, h2, cellPositionX);
            float h34 = MathHelper.Lerp(h3, h4, cellPositionX);

            float height = MathHelper.Lerp(h12, h34, cellPositionY);

            if (height < waterHeight) height = waterHeight;

            return height;
        }

        public override Vector3 GetNormal(Vector3 position) {
            Vector3 firstVertexPosition = vertices[0].Position;
            Vector3 relativePosition = position - firstVertexPosition;

            int gridMapPositionX = (int)(relativePosition.X / mapCellSize);
            int gridMapPositionY = (int)(relativePosition.Z / mapCellSize);
            int gridMapPositionNextX = gridMapPositionX + 1;
            int gridMapPositionNextY = gridMapPositionY + 1;

            if (gridMapPositionX == mapInfoWidth)
                gridMapPositionNextX -= 2;
            if (gridMapPositionNextY == mapInfoHeight)
                gridMapPositionNextY -= 2;

            float cellPositionX = relativePosition.X % mapCellSize / mapCellSize;
            float cellPositionY = relativePosition.Z % mapCellSize / mapCellSize;

            Vector3 v1 = vertices[gridMapPositionX + gridMapPositionY * mapInfoWidth].Normal;
            Vector3 v2 = vertices[gridMapPositionNextX + gridMapPositionY * mapInfoWidth].Normal;
            Vector3 v3 = vertices[gridMapPositionX + gridMapPositionNextY * mapInfoWidth].Normal;
            Vector3 v4 = vertices[gridMapPositionNextX + gridMapPositionNextY * mapInfoWidth].Normal;

            Vector3 v12 = Vector3.Lerp(v1, v2, cellPositionX);
            Vector3 v34 = Vector3.Lerp(v3, v4, cellPositionX);

            Vector3 normal = Vector3.Normalize(Vector3.Lerp(v12, v34, cellPositionY));

            return normal;
        }
    }
}
