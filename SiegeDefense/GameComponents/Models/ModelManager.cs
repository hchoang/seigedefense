﻿using Microsoft.Xna.Framework;
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
            TankController tankController = new TankController();

            //Camera camera = new FollowTargetCamera(userControlledTank, 50);
<<<<<<< HEAD
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 30, -80));
=======
            Camera camera = new TargetPointOfViewCamera(userControlledTank, new Vector3(0, 50, -100));
>>>>>>> 4c3d3489527167be4e6676b127fc6c529ab1fdf9
            Game.Components.Add(camera);
            userControlledTank.AddChild(tankController);
            models.Add(userControlledTank);
<<<<<<< HEAD
            models.Add(new Bullet(Game.Content.Load<Model>(@"Models/bullet"), userControlledTank.Position + new Vector3 (0, 0, 40)));
=======
            //models.Add(new Bullet(Game.Content.Load<Model>(@"Models/bullet"), userControlledTank.canonBone.Transform.Translation + userControlledTank.Position));
            models.Add(new Tank(Game.Content.Load<Model>(@"Models/tank")));
>>>>>>> 4c3d3489527167be4e6676b127fc6c529ab1fdf9
            base.LoadContent();
        }
    }
}
