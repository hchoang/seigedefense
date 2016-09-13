using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiegeDefense.GameScreens;
using SiegeDefense.GameComponents.Cameras;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Input;
using SiegeDefense.GameComponents.AI;
using SiegeDefense.GameComponents.Maps;
using Microsoft.Xna.Framework.Audio;
using SiegeDefense.GameComponents.SoundBank;
using SiegeDefense.GameComponents.Models;
using SiegeDefense.GameComponents.Sky;

namespace SiegeDefense.GameComponents
{
    public partial class GameManager : GameObject
    {
        // Models & sprites
        protected List<BaseModel> models = new List<BaseModel>();
        protected List<Tank> tankList = new List<Tank>();
        protected GameDetailSprite pointSprite;
        protected GameDetailSprite bloodSprite;
        protected Static2DSprite gameoverSprite;
        protected Tank userControlledTank;

        
        // Game mechanics
        protected int maxEnemy = 12;
        protected int spawnMaxAttempt = 50;
        protected float spawnCDTime = 10;
        protected float spawnCDCounter = 10;
        public int Point { get; set; }

        // Game world
        public Map map { get; protected set; }
        public Skybox sky { get; protected set; }
        public Camera mainCamera { get; protected set; }

        // display
        public SpriteBatch SpriteBatch { get; set; }
        public BasicEffect BasicEffect { get; set; }
        public Effect Effect { get; set; }
        public GraphicsDeviceManager deviceManager { get; set; }

        // sound & input
        protected SoundEffectInstance bgm;
        protected SoundBankManager soundManager;
        protected InputManager inputManager;

        public List<Tank> getTankList() {
            return tankList;
        }

        public void Add(BaseModel model) {
            //models.Add(model);
            Game.Components.Add(model);
            if (model is Tank) {
                tankList.Add((Tank)model);
            }
        }

        public void Remove(BaseModel model) {
            //models.Remove(model);
            Game.Components.Remove(model);
            if (model is Tank) {
                tankList.Remove((Tank)model);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BaseModel bm in models)
            {
                bm.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public void LoadLevel(string levelname) {

            // Clear all component
            Game.Components.Clear();

            // Add game world
            LevelDescription description = LevelDescription.LoadFromXML(Game.Content.RootDirectory + @"\level\" + levelname + ".xml");
            sky = new Skybox();
            map = new MultiTexturedHeightMap(description);
            Game.Components.Add(sky);
            Game.Components.Add(map);

            // Add player & camera
            userControlledTank = new Tank(Game.Content.Load<Model>(@"Models/tank"));
            userControlledTank.Position = map.SpawnPoints[0];
            userControlledTank.Tag = "Player";
            userControlledTank.AddChild(new TankController());
            Add(userControlledTank);
            mainCamera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(mainCamera);

            // Add game detail
            pointSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Point: " + 0, new Vector2(50, 50), Color.Green);
            bloodSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Blood: " + userControlledTank.blood, new Vector2(50, 100), Color.Green);
            gameoverSprite = new Static2DSprite(Game.Content.Load<Texture2D>(@"Sprites\GameOver"), Utility.CalculateDrawArea(Vector2.Zero, new Vector2(1, 1), Game.GraphicsDevice), Color.White);
            gameoverSprite.Visible = false;
            Game.Components.Add(bloodSprite);
            Game.Components.Add(pointSprite);
            Game.Components.Add(gameoverSprite);

            // Load sound & input & game manager
            soundManager = Game.Services.GetService<SoundBankManager>();
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
            Game.Components.Add((InputManager)Game.Services.GetService<IInputManager>());
            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            bgm.Play();
            if (userControlledTank.blood == 0) {
                gameoverSprite.Visible = true;
                return;
            }
            bloodSprite.setText("Blood: " + userControlledTank.blood);
            pointSprite.setText("Point: " + Point);
            
            // spawn enemy
            if (spawnCDCounter >= spawnCDTime) {
                spawnCDCounter = 0;
                if (tankList.Count() < maxEnemy) {
                    for (int i=0; i<spawnMaxAttempt; i++) {
                        Random r = new Random();
                        int spawnIndex = r.Next(map.SpawnPoints.Count);
                        Vector3 newTankLocation = map.SpawnPoints[spawnIndex];
                        newTankLocation.Y = map.GetHeight(newTankLocation);

                        Tank enemyTank = new AIControlledTank(Game.Content.Load<Model>(@"Models/tank"), newTankLocation, new TankAI(), userControlledTank);
                        enemyTank.Tag = "Enemy";
                        if (enemyTank.Moveable(newTankLocation)) {
                            Add(enemyTank);
                            Console.WriteLine(newTankLocation);
                            break;
                        }
                    }
                }
            }
            spawnCDCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < models.Count(); i++)
            {
                models[i].Update(gameTime);
            }
            base.Update(gameTime);
        }
    }
}