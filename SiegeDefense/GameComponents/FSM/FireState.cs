using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class FireState : State {

        public float FireCoolDownTime { get; set; } = 2;
        public float FireCoolDownCouter { get; set; } = 0;

        public override void PassiveUpdate(GameTime gameTime) {
            if (FireCoolDownCouter < FireCoolDownTime) {
                FireCoolDownCouter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void Update(GameTime gameTime) {

            if (FireCoolDownCouter >= FireCoolDownTime) {
                FireCoolDownCouter = 0;
                AIVehicle.Fire();
            }

            FireCoolDownCouter += (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}
