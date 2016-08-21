using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using System;

namespace SiegeDefense.GameComponents.Models
{
    class Tank : BaseModel
    {
        public ModelBone turretBone { get; protected set; }
        public ModelBone canonBone { get; protected set; }
        public Tank(Model model) : base(model)
        {
            turretBone = model.Bones["turret_geo"];
            canonBone = model.Bones["canon_geo"];
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Move(Vector3 moveDirection) {
            Vector3 newPosition = Position + moveDirection;
            if (!map.Moveable(newPosition))
                return;

            Vector3 mapNormal = map.GetNormal(newPosition);
            // angle between map normal & up vector -- calculate map slope
            float angle = MathHelper.Clamp(Vector3.Dot(mapNormal, Vector3.Up) / (mapNormal.Length()), -1, 1);
            angle = (float)Math.Acos(angle);
            // convert to degree
            angle = angle * 180 / MathHelper.Pi;

            if (Math.Abs(angle) > 45) return;

            //float newHeight = (bouding.Max.Y + bouding.Min.Y) / 2 + map.GetHeight(newPosition);
            float newHeight = map.GetHeight(newPosition);
            Position = new Vector3(newPosition.X, newHeight, newPosition.Z);
            Up = mapNormal;
        }
    }
}
