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

namespace SiegeDefense.GameComponents.Models
{
    public partial class ModelManager : GameObject
    {
        public List<BaseModel> models = new List<BaseModel>();
        public List<Tank> tankList = new List<Tank>();

        public ModelManager()
        {
            models = new List<BaseModel>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(BaseModel bm in models)
            {
                bm.Draw(gameTime);
            }

            foreach (Tank tank in tankList) {
                tank.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Update(gameTime);
            }
            foreach (Tank tank in tankList) {
                tank.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            Tank userControlledTank = new Tank(Game.Content.Load<Model>(@"Models/tank"));
            userControlledTank.Tag = "Player";
            userControlledTank.AddChild(new TankController());
            tankList.Add(userControlledTank);

            //Camera camera = new FollowTargetCamera(userControlledTank, 50);
            //Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 20, -5));
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(camera);

            Tank enemyTank = new AIControlledTank(Game.Content.Load<Model>(@"Models/tank"), new Vector3(500, 0, 600), new TankAI(), userControlledTank);
            tankList.Add(enemyTank);
            base.LoadContent();
        }
    }
}
