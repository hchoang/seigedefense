using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;

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

        public void Draw(FPSCamera camera)
        {
            Matrix[] transform = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transform);

            foreach(ModelMesh mess in model.Meshes)
            {
                foreach(BasicEffect effect in mess.Effects)
                {
                    effect.EnableDefaultLighting();

                }
            }
        }
    }
}
