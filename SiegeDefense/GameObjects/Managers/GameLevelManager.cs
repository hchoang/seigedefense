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
        protected int maxEnemy = 12;
        protected int spawnMaxAttempt = 50;
        protected float spawnCDTime = 1;
        protected float spawnCDCounter = 1;
        public int Point { get; set; }

        // Game world
        protected Map map { get; set; }
        protected Skybox sky { get; set; }
        protected Camera mainCamera { get; set; }
        protected OnlandVehicle userControlledTank;
        public Partition rootPartition { get; set; }

        public void LoadLevel(string levelname) {

            // Add game world
            LevelDescription description = LevelDescription.LoadFromXML(Game.Content.RootDirectory + @"\level\" + levelname + ".xml");
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
            pointSprite = new TextRenderer() { font = Game.Content.Load<SpriteFont>(@"Fonts\Arial"), text = "Point: " + 0, position = new Vector2(50, 50), color = Color.Green };
            bloodSprite = new TextRenderer() { font = Game.Content.Load<SpriteFont>(@"Fonts\Arial"), text = "HP: " + userControlledTank.HP, position = new Vector2(50, 100), color = Color.Green };
            gameoverSprite = new EmptyObject();
            gameoverSprite.AddComponent(new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\GameOver")));
            gameoverSprite.Visible = false;
            Game.Components.Add(bloodSprite);
            Game.Components.Add(pointSprite);
            Game.Components.Add(gameoverSprite);
        }

        public override void Update(GameTime gameTime) {
            if (userControlledTank.HP <= 0) {
                gameoverSprite.Visible = true;
                return;
            }
            bloodSprite.text = "HP: " + userControlledTank.HP;
            pointSprite.text = "Point: " + Point;

            // spawn enemy
            if (spawnCDCounter >= spawnCDTime) {
                spawnCDCounter = 0;
                List<OnlandVehicle> enemyTanks = FindObjectsByTag("Enemy").Cast<OnlandVehicle>().ToList();
                if (enemyTanks.Count < maxEnemy) {
                    for (int i = 0; i < spawnMaxAttempt; i++) {
                        Random r = new Random();
                        int spawnIndex = r.Next(map.SpawnPoints.Count);
                        Vector3 newTankLocation = map.SpawnPoints[spawnIndex];
                        newTankLocation.Y = map.GetHeight(newTankLocation);

                        OnlandVehicle enemyVehicle = VehicleFactory.CreateExplosiveTruck(ModelType.EXPLOSIVE_TRUCK1, 20);
                        enemyVehicle.Tag = "Enemy";
                        enemyVehicle.transformation.Position = newTankLocation;
                        enemyVehicle.AddToGameWorld();
                        if (enemyVehicle.Moveable(newTankLocation)) {
                            enemyVehicle.AddComponent(new ExplosiveTruckAI());
                            break;
                        } else {
                            enemyVehicle.RemoveFromGameWorld();
                        }
                    }
                }
            }
            spawnCDCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}
