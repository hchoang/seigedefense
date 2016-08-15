using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Physics;
using SiegeDefense.GameComponents.Sky;

namespace SiegeDefense.GameScreens {
    public class MainGameScreen : GameObject {

        private Skybox skyBox;
        private MultiTexturedHeightMap map;
        private FPSCamera camera;

        private BasicEffect basicEffect;
        private Effect advancedEffect;

        public MainGameScreen() {

            basicEffect = Game.Services.GetService<BasicEffect>();
            advancedEffect = Game.Services.GetService<Effect>();

            skyBox = new Skybox();
            map = new MultiTexturedHeightMap(10, 100);
            Vector3 cameraPosition = new Vector3(500, 0, 500);
            cameraPosition = new Vector3(cameraPosition.X, map.GetHeight(cameraPosition), cameraPosition.Z);
            camera = new FPSCamera(cameraPosition, new Vector3(100, 20, 100), Vector3.Up);

            Game.Components.Add(skyBox);
            Game.Components.Add(map);
            Game.Components.Add(camera);

            skyBox.GetDependentComponents();
            map.GetDependentComponents();
            camera.GetDependentComponents();
        }


        public override void Update(GameTime gameTime) {
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            advancedEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            advancedEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);

            base.Update(gameTime);
        }
    }
}
