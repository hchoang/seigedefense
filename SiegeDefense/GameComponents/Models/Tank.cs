using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using System;

namespace SiegeDefense.GameComponents.Models
{
    class Tank : BaseModel
    {
        public Tank(Model model) : base(model)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Move(Vector3 moveDirection) {
            Vector3 newPosition = Position + moveDirection;
            if (!map.Moveable(newPosition))
                return;

            //float newHeight = (bouding.Max.Y + bouding.Min.Y) / 2 + map.GetHeight(newPosition);
            float newHeight = map.GetHeight(newPosition);

            Position = new Vector3(newPosition.X, newHeight, newPosition.Z);
            Vector3 mapNormal = map.GetNormal(Position);
            Up = mapNormal;
        }
    }
}
