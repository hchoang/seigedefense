using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Models;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Maps;

namespace SiegeDefense.GameComponents.AI
{
    class TankAI : GameObject
    {
        private Vector3 lastEnemyPosition;
        private float tankMoveSpeed = 30f;
        private float tankRotaionSpeed = 1f;
        private float turretRotateSpeed = 0.05f;
        private Vector3 tankTarget = Vector3.Zero;
        BasicEffect basicEffect;
        Vector3 steeringForce = Vector3.Zero;
        private float timer = 0;
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

        private Map _map;
        private Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        
        public override void Draw(GameTime gameTime) {
            // draw steering force
            Vector3 yOffset = new Vector3(0, 20, 0);
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0].Position = AITank.Position + yOffset;
            vertices[0].Color = Color.Red;
            vertices[1].Position = AITank.Position + Vector3.Normalize(steeringForce) * 50 + yOffset;
            vertices[1].Color = Color.Red;

            int[] indices = new int[2] { 0, 1 };

            if (basicEffect == null)
                basicEffect = (BasicEffect)Game.Services.GetService<BasicEffect>().Clone();

            Camera camera = FindObjects<Camera>()[0];
            basicEffect.EnableDefaultLighting();
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.FogEnabled = false;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 2, indices, 0, 1);
            }

            base.Draw(gameTime);
        }

        float wanderingChangeTime = 0;
        float wanderingChangeCounter = 0;
        public override void Update(GameTime gameTime)
        {
            AITank.RotateWheels(-1);
            if (AITank.isInVisibleRange(AITank.enemy))
            {
                wanderingChangeCounter = wanderingChangeTime;
                steeringForce = TankBehaviour.ChaseTargetBehaviour(AITank.WorldMatrix, AITank.enemy.WorldMatrix);
            }
            else
            {
                wanderingChangeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (wanderingChangeCounter >= wanderingChangeTime)
                {
                    wanderingChangeCounter = 0;
                    Random r = new Random();
                    wanderingChangeTime = r.Next(3, 6);
                    steeringForce = TankBehaviour.WanderingBehaviour(AITank.WorldMatrix);
                }
            }

            if (steeringForce != Vector3.Zero) {
                steeringForce = TankBehaviour.AdvoidObstacleBehaviour(AITank, steeringForce, map, 50);
                steeringForce = TankBehaviour.AdvoidObstacleBehaviour(AITank, steeringForce, map, tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (steeringForce != Vector3.Zero) {

                // project steering force to plane (Forward, Left)
                Vector3 normalizedUp = Vector3.Normalize(AITank.Up);
                float t = Vector3.Dot(normalizedUp, steeringForce);
                steeringForce = steeringForce - t * normalizedUp;

                float steeringAngle = Utility.RotationAngleCalculator(AITank.Forward, steeringForce, AITank.Left);
                if (steeringAngle != 0) {
                    float steeringDirection = steeringAngle > 0 ? 1 : -1;
                    float tankRotation = steeringDirection * tankRotaionSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Math.Abs(tankRotation) > Math.Abs(steeringAngle)) {
                        tankRotation = steeringAngle;
                    }
                    AITank.RotateTank(tankRotation);
                }
                bool moved = AITank.Move(Vector3.Normalize(AITank.Forward) * tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            
            if (AITank.isInFireRange(AITank.enemy) && isAimed())
            {
                timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timer > 1000)
                {
                    AITank.Fire();
                    timer = 0;
                }
            }



            //lastEnemyPosition = AITank.enemy.Position;
            //if (AITank.isInRange(AITank.enemy)) {
            //Vector3 newForward = Vector3.Normalize(AITank.enemy.Position - AITank.Position);
            //float turretRotationAngle = Utility.RotationAngleCalculator(AITank.Forward, newForward, AITank.Left);

            //    if (!AITank.RotateTurret(-turretRotationAngle * (float)gameTime.ElapsedGameTime.TotalSeconds))
            //    {
            //        Vector3 newTankForward = Vector3.Normalize(AITank.enemy.Position - AITank.Position);
            //float rotationAngle = Utility.RotationAngleCalculator(AITank.Forward, newForward, AITank.Left);
            //Console.Out.WriteLine(rotationAngle);
            //        Vector3 moveDirection = Vector3.Zero;

            //        moveDirection += AITank.Forward;

            //        moveDirection = Vector3.Normalize(moveDirection) * tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //        rotationAngle = rotationAngle * tankRotaionSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //        AITank.Move(moveDirection);
            //        AITank.RotateTank(rotationAngle);
            //        AITank.RotateWheels(-1);
            //    }

            //}

            base.Update(gameTime);
        }

        public List<Vector3> PathFinding()
        {
            return null;
        }

        public bool isAimed()
        {
            if (Math.Abs(Utility.RotationAngleCalculator(AITank.Forward + AITank.Position, AITank.enemy.Position, AITank.Left)) <= 0.2)
            {
                return true;
            }
            return false;
        }
    }
}
