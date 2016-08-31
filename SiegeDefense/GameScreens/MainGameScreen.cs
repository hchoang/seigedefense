using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Physics;
using SiegeDefense.GameComponents.Sky;
using SiegeDefense.GameComponents.Models;
using System;
using SiegeDefense.GameComponents.SoundBank;
using SiegeDefense.GameComponents.TitleScreen;

namespace SiegeDefense.GameScreens {
    public class MainGameScreen : GameObject {

        private Skybox skyBox;
        private MultiTexturedHeightMap map;
        //private FPSCamera camera;
        private SoundBankManager soundManager;

        private ModelManager modelManager;
        private GameDetailSprite pointSprite;
        private GameDetailSprite bloodSprite;


        public MainGameScreen() {

            skyBox = new Skybox();
            modelManager = new ModelManager();
            soundManager = new SoundBankManager();
            pointSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Point: 0", new Vector2(50, 50), Color.Green);
            bloodSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Blood: 100", new Vector2(50, 100), Color.Green);

            map = new MultiTexturedHeightMap(10, 200);
            Vector3 cameraPosition = new Vector3(500, 0, 500);
            cameraPosition = new Vector3(cameraPosition.X, map.GetHeight(cameraPosition), cameraPosition.Z);
            //camera = new FPSCamera(cameraPosition, new Vector3(100, 20, 100), Vector3.Up);
            
            Game.Components.Add(skyBox);
            Game.Components.Add(map);
            Game.Components.Add(modelManager);
            Game.Components.Add(soundManager);
            Game.Components.Add(pointSprite);
            Game.Components.Add(bloodSprite);


            //Vector3 position = new Vector3(100, 0, 100);
            //Matrix test = Matrix.CreateWorld(position, Vector3.Normalize(new Vector3(10, 0, 10)), Vector3.Up);
            //int dsgdsg = 1;
        }
    }
}
