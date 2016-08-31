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

namespace SiegeDefense.GameComponents.Models
{
    public partial class ModelManager : GameObject
    {
        protected List<BaseModel> models = new List<BaseModel>();
        protected List<Tank> tankList = new List<Tank>();
        protected List<Vector3> spawnPoints = new List<Vector3>();
        protected Tank userControlledTank;
        protected int maxEnemy = 20;
        protected int spawnMaxAttempt = 50;
        protected float spawnCDTime = 3;
        protected float spawnCDCounter = 3;
        
        private Map _map;
        private Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public ModelManager()
        {
            models = new List<BaseModel>();
            spawnPoints.Add(new Vector3(580, 0, 292));
            spawnPoints.Add(new Vector3(830, 0, 737));
            spawnPoints.Add(new Vector3(1207, 0, 835));
            spawnPoints.Add(new Vector3(1180, 0, 1210));
            spawnPoints.Add(new Vector3(700, 0, 1221));
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
            if (spawnCDCounter >= spawnCDTime) {
                spawnCDCounter = 0;
                if (tankList.Count() < maxEnemy) {
                    for (int i=0; i<spawnMaxAttempt; i++) {
                        Random r = new Random();
                        int spawnIndex = r.Next(spawnPoints.Count);
                        Vector3 newTankLocation = spawnPoints[spawnIndex];
                        newTankLocation.Y = map.GetHeight(newTankLocation);

                        Tank enemyTank = new AIControlledTank(Game.Content.Load<Model>(@"Models/tank"), newTankLocation, new TankAI(), userControlledTank);
                        enemyTank.Tag = "Enemy";
                        if (enemyTank.Moveable(newTankLocation)) {
                            Add(enemyTank);
                            Console.WriteLine(newTankLocation);
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
            userControlledTank = new Tank(Game.Content.Load<Model>(@"Models/tank"));
            userControlledTank.Tag = "Player";
            userControlledTank.AddChild(new TankController());
            Add(userControlledTank);

            //Camera camera = new FollowTargetCamera(userControlledTank, 50);
            //Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 20, -5));
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(camera);
            
            base.LoadContent();
        }
    }
}
