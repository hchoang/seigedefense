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
                bm.Draw();
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Update();
            }
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            models.Add(new Tank(Game.Content.Load<Model>(@"Models/tank"), GameObject.FindObjects<FPSCamera>()[0]));
            base.LoadContent();
        }
    }
}
