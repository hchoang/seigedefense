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
        private GamePhysics cameraPhysics;

        private BasicEffect basicEffect;
        private Effect advancedEffect;

        public MainGameScreen() {

            basicEffect = Game.Services.GetService<BasicEffect>();
            advancedEffect = Game.Services.GetService<Effect>();

            skyBox = new Skybox();
            map = new MultiTexturedHeightMap(10, 100);
            camera = new FPSCamera(new Vector3(500, 200, 500), new Vector3(100, 20, 100), Vector3.Up);
            cameraPhysics = new GamePhysics();

            skyBox.camera = camera;

            cameraPhysics.map = map;
            cameraPhysics.Target = camera;

            Game.Components.Add(skyBox);
            Game.Components.Add(map);
            Game.Components.Add(camera);

            skyBox.camera = camera;
            camera.map = map;
        }

        public override void Update(GameTime gameTime) {
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            advancedEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            advancedEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
        }
    }
}
