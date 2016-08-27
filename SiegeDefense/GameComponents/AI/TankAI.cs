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
        private float tankRotaionSpeed = 2f;
        protected float turretRotateSpeed = 0.05f;
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
            lastEnemyPosition = AITank.enemy.Position;
            if (AITank.isInRange(AITank.enemy)) {
                Vector3 newForward = Vector3.Normalize(AITank.enemy.Position - AITank.Position);
                float turretRotationAngle = Utility.RotationAngleCalculator(AITank.Forward, newForward, AITank.Left);
                Console.Out.WriteLine(turretRotationAngle);
                if (!AITank.RotateTurret(-turretRotationAngle * (float)gameTime.ElapsedGameTime.TotalSeconds))
                {
                    Vector3 newTankForward = Vector3.Normalize(AITank.enemy.Position - AITank.Position);
                    float rotationAngle = Utility.RotationAngleCalculator(AITank.Forward, newForward, AITank.Left);
                    Vector3 moveDirection = Vector3.Zero;

                    moveDirection += AITank.Forward;

                    moveDirection = Vector3.Normalize(moveDirection) * tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    rotationAngle = rotationAngle * tankRotaionSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    AITank.Move(moveDirection);
                    AITank.RotateTank(rotationAngle);
                    AITank.RotateWheels(-1);
                }
                
            }

            base.Update(gameTime);
        }

        public List<Vector3> PathFinding()
        {
            return null;
        }
    }
}
