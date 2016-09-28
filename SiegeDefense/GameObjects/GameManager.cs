using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiegeDefense {
    public partial class GameManager : GameObject
    {
        // Game details
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
        protected Map map { get; set; }
        protected Skybox sky { get; set; }
        protected Camera mainCamera { get; set; }

        // display
        public SpriteBatch SpriteBatch { get; set; }
        public BasicEffect BasicEffect { get; set; }
        public Effect Effect { get; set; }
        public GraphicsDeviceManager deviceManager { get; set; }

        // sound & input
        protected SoundEffectInstance bgm;
        protected SoundBankManager soundManager;
        protected InputManager inputManager;

        public void LoadLevel(string levelname) {
            // Clear all components
            Game.Components.Clear();

            // Add game world
            LevelDescription description = LevelDescription.LoadFromXML(Game.Content.RootDirectory + @"\level\" + levelname + ".xml");
            sky = new Skybox();
            map = new HeightMap(description);
            Game.Components.Add(sky);
            Game.Components.Add(map);

            // Add player & camera
            userControlledTank = new Tank(ModelType.TANK1);
            userControlledTank.transformation.Position = map.PlayerStartPosition;
            userControlledTank.Tag = "Player";
            userControlledTank.AddComponent(new TankController());
            Game.Components.Add(userControlledTank);
            mainCamera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(mainCamera);

            // Add game detail
            pointSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Point: " + 0, new Vector2(50, 50), Color.Green);
            bloodSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "HP: " + userControlledTank.HP, new Vector2(50, 100), Color.Green);
            gameoverSprite = new Static2DSprite(Game.Content.Load<Texture2D>(@"Sprites\GameOver"), Utility.CalculateDrawArea(Vector2.Zero, new Vector2(1, 1), Game.GraphicsDevice), Color.White);
            gameoverSprite.Visible = false;
            Game.Components.Add(bloodSprite);
            Game.Components.Add(pointSprite);
            Game.Components.Add(gameoverSprite);

            // Load sound & input & game manager
            soundManager = Game.Services.GetService<SoundBankManager>();
            Game.Components.Add((InputManager)Game.Services.GetService<IInputManager>());
            Game.Components.Add(this);

            // play BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public override void Update(GameTime gameTime)
        {
            bgm.Play();
            if (userControlledTank.HP <= 0) {
                gameoverSprite.Visible = true;
                return;
            }
            bloodSprite.setText("HP: " + userControlledTank.HP);
            pointSprite.setText("Point: " + Point);
            
            // spawn enemy
            if (spawnCDCounter >= spawnCDTime) {
                spawnCDCounter = 0;
                List<Tank> enemyTanks = FindObjectsByTag("Enemy").Cast<Tank>().ToList();
                if (enemyTanks.Count() < maxEnemy) {
                    for (int i=0; i<spawnMaxAttempt; i++) {
                        Random r = new Random();
                        int spawnIndex = r.Next(map.SpawnPoints.Count);
                        Vector3 newTankLocation = map.SpawnPoints[spawnIndex];
                        newTankLocation.Y = map.GetHeight(newTankLocation);

                        Tank enemyTank = new AIControlledTank(ModelType.TANK1, newTankLocation, new TankAI(), userControlledTank);
                        enemyTank.Tag = "Enemy";
                        if (enemyTank.Moveable(newTankLocation)) {
                            Game.Components.Add(enemyTank);
                            break;
                        }
                    }
                }
            }
            spawnCDCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}