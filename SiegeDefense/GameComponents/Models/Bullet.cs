using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SiegeDefense.GameComponents.Models
{
    class Bullet : BaseModel
    {
        public Bullet(Model model, Vector3 Position): base(model, Position)
        {
            ScaleMatrix = Matrix.CreateScale(1);
        }
    }
}
