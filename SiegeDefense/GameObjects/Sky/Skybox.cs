using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class Skybox : _3DGameObject {
        public new SkyRenderer renderer { get; protected set; }

        public Skybox() {
            Tag = "Sky";

            renderer = new SkyRenderer();
            AddComponent(renderer);

            transformation = new Transformation();
            transformation.ScaleMatrix = Matrix.CreateScale(500);
        }

        public override void Update(GameTime gameTime) {
            transformation.Position = mainCamera.Position;

            base.Update(gameTime);
        }
    }
}
