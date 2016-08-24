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

namespace SiegeDefense.GameComponents.Models
{
    public partial class ModelManager : GameObject
    {
        private List<BaseModel> models = new List<BaseModel>();

        public ModelManager()
        {
            models = new List<BaseModel>();
            //InitializeComponent();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach(BaseModel bm in models)
            {
                bm.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            Tank userControlledTank = new Tank(Game.Content.Load<Model>(@"Models/tank"));
            Console.Out.WriteLine(3);
            TankController tankController = new TankController();

            //Camera camera = new FollowTargetCamera(userControlledTank, 50);
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(40, 50, -100));
            Game.Components.Add(camera);

            userControlledTank.AddChild(tankController);
            models.Add(userControlledTank);
            models.Add(new Bullet(Game.Content.Load<Model>(@"Models/bullet"), userControlledTank.canonBone.Transform.Translation + userControlledTank.Position));
            base.LoadContent();
        }
    }
}
