using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class AimState : AIState {

        public override void Update(GameTime gameTime) {
            Vector3 target = player.transformation.Position - AITank.transformation.Position;

            float upAngle = Utility.RotationAngleCalculator(AITank.transformation.Up, target, AITank.transformation.Left);
            
            if (Math.Abs(Math.Abs(upAngle) - MathHelper.PiOver2) > 0.1f) {
                AI.steeringForce = AITank.transformation.Forward;
                return;
            }

            AI.steeringForce = Vector3.Zero;
            float angle = Utility.RotationAngleCalculator(AITank.transformation.Forward, target, AITank.transformation.Left);

            if (Math.Abs(angle) < 0.1f) {
                AITank.physics.RotateForce = 0;
                AITank.physics.RotateAcceleration = 0;
                return;
            }

            AITank.physics.RotateForce = angle > 0 ? 0.1f : -0.1f;

            base.Update(gameTime);
        }
    }
}
