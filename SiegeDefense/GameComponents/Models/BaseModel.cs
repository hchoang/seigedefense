using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense.GameComponents.Models
{
    class BaseModel
    {
        public Model model { get; protected set; }
        protected Matrix world;

        public BaseModel(Model model)
        {
            this.model = model;
        }

        protected virtual void Update()
        {

        }
    }
}
