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
        protected List<BaseModel> models = new List<BaseModel>();
        protected List<Tank> tankList = new List<Tank>();
        protected List<Vector3> spawnPoints = new List<Vector3>();
        protected GameDetailSprite pointSprite;
        protected GameDetailSprite bloodSprite;
        protected UserControlledTank userControlledTank;

        public ModelManager()
        {
            models = new List<BaseModel>();
            userControlledTank = new UserControlledTank(Game.Content.Load<Model>(@"Models/tank"));
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
            bloodSprite.setText("Blood: " + userControlledTank.blood);
            pointSprite.setText("Point: " + userControlledTank.point);
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            userControlledTank.Tag = "Player";
            userControlledTank.AddChild(new TankController());
            Add(userControlledTank);

            pointSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Point: " + 0, new Vector2(50, 50), Color.Green);
            bloodSprite = new GameDetailSprite(Game.Content.Load<SpriteFont>(@"Fonts\Arial"), "Blood: " + userControlledTank.blood, new Vector2(50, 100), Color.Green);

            //Camera camera = new FollowTargetCamera(userControlledTank, 50);
            //Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 20, -5));
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, 100));
            Game.Components.Add(camera);
            Game.Components.Add(bloodSprite);
            Game.Components.Add(pointSprite);

            Tank enemyTank = new AIControlledTank(Game.Content.Load<Model>(@"Models/tank"), new Vector3(500, 0, 400), new TankAI(), userControlledTank);
            Add(enemyTank);
            base.LoadContent();
        }
    }
}
