using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Models;

namespace SiegeDefense.GameComponents.AI
{
    class TankAI : GameObject
    {
        private Vector3 lastEnemyPosition;
        private float tankMoveSpeed = 20f;
        public AIControlledTank _AITank;
        public AIControlledTank AITank
        {
            get
            {
                if (_AITank == null)
                {
                    _AITank =  FindComponent<AIControlledTank>();
                }
                return _AITank;
            }
        }

        
        public override void Update(GameTime gameTime)
        {
            if (AITank.isInRange(AITank.enemy)) {
                lastEnemyPosition = AITank.enemy.Position;
                Vector3 newForward = AITank.enemy.Position - AITank.Position;
                float rotationAngle = Utility.RotationAngleCalculator(AITank.Forward, newForward, AITank.Left);

                Vector3 moveDirection = Vector3.Zero;
                
                moveDirection += AITank.Forward;
                
                if (moveDirection != Vector3.Zero)
                {
                    moveDirection = Vector3.Normalize(moveDirection) * tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    AITank.Move(moveDirection);
                    AITank.RotateTank(rotationAngle);
                }
                
            }
            base.Update(gameTime);
        }
    }
}
