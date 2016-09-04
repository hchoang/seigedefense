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

namespace SiegeDefense.GameComponents.Models
{
    public partial class ModelManager : GameObject
    {
        protected List<BaseModel> models = new List<BaseModel>();
        protected List<Tank> tankList = new List<Tank>();
        protected List<Vector3> spawnPoints = new List<Vector3>();
        protected GameDetailSprite pointSprite;
        protected GameDetailSprite bloodSprite;
        protected Static2DSprite gameoverSprite;
        protected UserControlledTank userControlledTank;
        protected SoundEffectInstance bgm;
        protected SoundBankManager soundManager;

        protected int maxEnemy = 12;
        protected int spawnMaxAttempt = 50;
        protected float spawnCDTime = 10;
        protected float spawnCDCounter = 10;
        
        private Map _map;
        private Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public List<Tank> getTankList() {
            return tankList;
        }

        public void Add(BaseModel model) {
            models.Add(model);
            if (model is Tank) {
                tankList.Add((Tank)model);
            }
        }

        public void Remove(BaseModel model) {
            models.Remove(model);
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

        public override void Update(GameTime gameTime)
        {
            bgm.Play();
            if (userControlledTank.blood == 0) {
                gameoverSprite.Visible = true;
                return;
            }
            bloodSprite.setText("Blood: " + userControlledTank.blood);
            pointSprite.setText("Point: " + userControlledTank.point);
            
            // spawn enemy
            if (spawnCDCounter >= spawnCDTime) {
                spawnCDCounter = 0;
                if (tankList.Count() < maxEnemy) {
                    for (int j=0; j<spawnMaxAttempt; j++) {
                        Random r = new Random();
                        int spawnIndex = r.Next(spawnPoints.Count);
                        Vector3 newTankLocation = spawnPoints[spawnIndex];
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

        protected override void LoadContent()
        {
            soundManager = Game.Services.GetService<SoundBankManager>();
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;

            spawnPoints.Add(new Vector3(580, 0, 292));
            spawnPoints.Add(new Vector3(830, 0, 738));
            spawnPoints.Add(new Vector3(1207, 0, 835));
            spawnPoints.Add(new Vector3(1180, 0, 1210));
            spawnPoints.Add(new Vector3(700, 0, 1221));

            for (int i=0; i<spawnPoints.Count; i++) {
                float height = map.GetHeight(spawnPoints[i]);
                spawnPoints[i] = new Vector3(spawnPoints[i].X, height, spawnPoints[i].Z);
            }

            userControlledTank = new UserControlledTank(Game.Content.Load<Model>(@"Models/tank"));
            userControlledTank.Position = spawnPoints[0];

            userControlledTank.Tag = "Player";
            userControlledTank.AddChild(new TankController());
            Add(userControlledTank);

            pointSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Point: " + 0, new Vector2(50, 50), Color.Green);
            bloodSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Blood: " + userControlledTank.blood, new Vector2(50, 100), Color.Green);

            gameoverSprite = new Static2DSprite(Game.Content.Load<Texture2D>(@"Sprites\GameOver"), Utility.CalculateDrawArea(Vector2.Zero, new Vector2(1, 1), Game.Services.GetService<GraphicsDeviceManager>()), Color.White);
            gameoverSprite.Visible = false;

            //Camera camera = new FollowTargetCamera(userControlledTank, 50);
            //Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 20, -5));
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(camera);
            Game.Components.Add(bloodSprite);
            Game.Components.Add(pointSprite);
            Game.Components.Add(gameoverSprite);

            base.LoadContent();
        }
    }
}
