using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiegeDefense {
    public class GameLevelManager : GameObject {
        // Game details
        protected TextRenderer pointSprite;
        protected TextRenderer bloodSprite;
        protected EmptyObject gameoverSprite;

        // Game mechanics
        protected int maxEnemy = 20;
        protected int spawnMaxAttempt = 50;
        protected float spawnCDTime = 5;
        protected float spawnCDCounter = 5;
        public int Point { get; set; }

        // Game world
        protected Map map { get; set; }
        protected Skybox sky { get; set; }
        protected Camera mainCamera { get; set; }
        protected OnlandVehicle userControlledTank;
        public Partition rootPartition { get; set; }
        public IInputManager inputManager { get; set; }

        public void LoadLevel(LevelDescription description) {

            // Add game world
            sky = new Skybox();
            map = new HeightMap(description);
            Game.Components.Add(sky);
            Game.Components.Add(map);

            rootPartition = new Partition(map);
            Game.Components.Add(rootPartition);

            // Add player & camera
            userControlledTank = VehicleFactory.CreateTank(modelType: ModelType.TANK1, HP: 200);
            userControlledTank.transformation.Position = map.PlayerStartPosition;
            userControlledTank.Tag = "Player";
            userControlledTank.AddComponent(new TankController());
            userControlledTank.AddToGameWorld();
            mainCamera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(mainCamera);

            // Add game detail
            pointSprite = new TextRenderer() { font = Game.Content.Load<SpriteFont>(@"Fonts\Arial"), text = "Point: " + 0, position = new Vector2(50, 50), color = Color.Red };
            bloodSprite = new TextRenderer() { font = Game.Content.Load<SpriteFont>(@"Fonts\Arial"), text = "HP: " + userControlledTank.HP, position = new Vector2(50, 100), color = Color.Red };
            
            Game.Components.Add(bloodSprite);
            Game.Components.Add(pointSprite);

            inputManager = Game.Services.GetService<IInputManager>();
        }

        public void ExitButtonClick(HUD invoker) {
            inputManager.toggleCursor(true);
            gameManager.LoadTitleScreen();
        }
            
        public void ResumeButtonClick(HUD invoker) {
            gameManager.isPaused = false;
            inputManager.toggleCursor(false);
        }

        public void ShowGameOverScreen() {
            if (gameoverSprite == null) {
                gameoverSprite = new EmptyObject();
                gameoverSprite.AddComponent(new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\GameOver")));
                Game.Components.Add(gameoverSprite);

                // exit button
                _2DRenderer exitButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
                exitButtonRenderer.AddChildRenderer(new TextRenderer("Exit"));
                exitButtonRenderer.position = new Vector2(0.45f, 0.65f);
                exitButtonRenderer.size = new Vector2(0.1f, 0.1f);

                HUD exitButton = new HUD(exitButtonRenderer);
                exitButton.onClick = ExitButtonClick;
                Game.Components.Add(exitButton);

                inputManager.toggleCursor(true);
            }
        }

        public void DisplayPauseGameScreen() {
            // modal background
            _2DRenderer modalBackgroundRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\WhiteBar"));
            modalBackgroundRenderer.color = Color.Black * 0.5f;
            HUD modalBackground = new HUD(modalBackgroundRenderer);
            modalBackground.Tag = "Modal";
            Game.Components.Add(modalBackground);

            // resume button
            _2DRenderer resumeButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            resumeButtonRenderer.AddChildRenderer(new TextRenderer("Resume"));
            resumeButtonRenderer.position = new Vector2(0.35f, 0.5f);
            resumeButtonRenderer.size = new Vector2(0.1f, 0.1f);

            HUD resumeButton = new HUD(resumeButtonRenderer);
            resumeButton.Tag = "Modal";
            resumeButton.onClick = ResumeButtonClick;
            Game.Components.Add(resumeButton);

            // exit button
            _2DRenderer exitButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            exitButtonRenderer.AddChildRenderer(new TextRenderer("Exit"));
            exitButtonRenderer.position = new Vector2(0.55f, 0.5f);
            exitButtonRenderer.size = new Vector2(0.1f, 0.1f);

            HUD exitButton = new HUD(exitButtonRenderer);
            exitButton.Tag = "Modal";
            exitButton.onClick = ExitButtonClick;
            Game.Components.Add(exitButton);
        }

        public override void Update(GameTime gameTime) {
            if (userControlledTank.HP <= 0) {
                ShowGameOverScreen();
                List<GameObject> enemies = FindObjectsByTag("Enemy");
                foreach (GameObject enemy in enemies) {
                    Game.Components.Remove(enemy);
                }
                return;
            }

            if (inputManager.isTriggered(GameInput.Pause)) {
                gameManager.isPaused = !gameManager.isPaused;
                if (gameManager.isPaused) {
                    DisplayPauseGameScreen();
                    inputManager.toggleCursor(true);
                } else {
                    inputManager.toggleCursor(false);
                }
            }

            bloodSprite.text = "HP: " + userControlledTank.HP;
            pointSprite.text = "Point: " + Point;

            // spawn enemy
            if (spawnCDCounter >= spawnCDTime) {
                spawnCDCounter = 0;
                List<OnlandVehicle> enemyTanks = FindObjectsByTag("Enemy").Cast<OnlandVehicle>().ToList();
                if (enemyTanks.Count < maxEnemy) {
                    Random r = new Random();
                    int randomeValue = r.Next(0, 100);
                    if (randomeValue > 30) {
                        SpawnEnemyTank();
                    } else {
                        SpawnEnemyTruck();
                    }
                }
            }
            spawnCDCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public void SpawnEnemyTruck() {
            for (int i = 0; i < spawnMaxAttempt; i++) {
                Random r = new Random();
                int spawnIndex = r.Next(map.SpawnPoints.Count);
                Vector3 newTruckLocation = map.SpawnPoints[spawnIndex];
                newTruckLocation.Y = map.GetHeight(newTruckLocation);

                OnlandVehicle enemyVehicle = VehicleFactory.CreateExplosiveTruck(ModelType.EXPLOSIVE_TRUCK1, 20);
                enemyVehicle.Tag = "Enemy";
                enemyVehicle.transformation.Position = newTruckLocation;
                enemyVehicle.AddToGameWorld();
                if (enemyVehicle.Moveable(newTruckLocation)) {
                    enemyVehicle.AddComponent(new ExplosiveTruckAI());
                    break;
                } else {
                    enemyVehicle.RemoveFromGameWorld();
                }
            }
        }

        public void SpawnEnemyTank() {
            for (int i = 0; i < spawnMaxAttempt; i++) {
                Random r = new Random();
                int spawnIndex = r.Next(map.SpawnPoints.Count);
                Vector3 newTankLocation = map.SpawnPoints[spawnIndex];
                newTankLocation.Y = map.GetHeight(newTankLocation);

                OnlandVehicle enemyVehicle = VehicleFactory.CreateTank(ModelType.TANK1, 60);
                enemyVehicle.Tag = "Enemy";
                enemyVehicle.transformation.Position = newTankLocation;
                enemyVehicle.AddToGameWorld();
                if (enemyVehicle.Moveable(newTankLocation)) {
                    enemyVehicle.AddComponent(new EnemyTankAI());
                    break;
                } else {
                    enemyVehicle.RemoveFromGameWorld();
                }
            }
        }
    }

}
