using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Physics;
using System;

namespace SiegeDefense.GameComponents.Models
{
    public class Tank : BaseModel
    {
        protected int turretBoneIndex;
        protected int canonBoneIndex;
        protected int canonHeadBoneIndex;
        protected int[] wheelBoneIndex;
        public float turretMaxRotaion { get { return 0.2f; } private set { } }

        public Tank(Model model) : base(model)
        {
            wheelBoneIndex = new int[4];

            wheelBoneIndex[0] = model.Bones["l_front_wheel_geo"].Index;
            wheelBoneIndex[1] = model.Bones["r_front_wheel_geo"].Index;
            wheelBoneIndex[2] = model.Bones["l_back_wheel_geo"].Index;
            wheelBoneIndex[3] = model.Bones["r_back_wheel_geo"].Index;

            turretBoneIndex = model.Bones["turret_geo"].Index;  
            canonBoneIndex = model.Bones["canon_geo"].Index;
            canonHeadBoneIndex = model.Bones["canon_head_geo"].Index;   
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Move(Vector3 moveDirection) {
            Vector3 oldPosition = Position;
            Vector3 newPosition = Position + moveDirection;

            // validate new position on map
            if (!map.IsInsideMap(newPosition))
                return;

            // update new position height
            newPosition.Y = map.GetHeight(newPosition);

            if (!map.IsAccessibleByFoot(newPosition))
                return;

            Vector3 mapNormal = map.GetNormal(newPosition);
            float angle = MathHelper.Clamp(Vector3.Dot(mapNormal, Vector3.Up) / (mapNormal.Length()), -1, 1);
            angle = (float)Math.Acos(angle);
            angle = angle * 180 / MathHelper.Pi;

            if (Math.Abs(angle) > 45) return;

            Position = newPosition;

            // collision check with other tanks
            foreach (Tank tank in modelManager.getTankList()) {
                if (tank == this) continue;
                if (collisionBox.SphereIntersect(tank.collisionBox)) {
                    Position = oldPosition;
                    return;
                }
            }

            Up = mapNormal;

            Console.Out.WriteLine(Position);
        }

        public void RotateCanon(float rotationAngle)
        {
            Matrix canonMatrix = relativeTransform[canonBoneIndex];
            Vector3 canonForward = canonMatrix.Forward;
            Vector3 canonPosition = canonMatrix.Translation;
            Vector3 canonUp = canonMatrix.Up;

            Matrix canonRotateMatrix = Matrix.CreateFromAxisAngle(canonMatrix.Left, rotationAngle);
            canonForward = Vector3.Transform(canonForward, canonRotateMatrix);
            canonForward.Normalize();
            if (-0.2f < canonForward.Y && canonForward.Y < 0.5f)
            {
                canonUp = Vector3.Transform(canonUp, canonRotateMatrix);
                relativeTransform[canonBoneIndex] = Matrix.CreateWorld(canonPosition, canonForward, canonUp);
            }
        }

        public bool RotateTurret(float rotationAngle)
        {
            Matrix turretMatrix = relativeTransform[turretBoneIndex];
            Vector3 turretForward = turretMatrix.Forward;
            Vector3 turretPosition = turretMatrix.Translation;
            Vector3 turretUp = turretMatrix.Up;

            Matrix turretRotateMatrix = Matrix.CreateFromAxisAngle(turretMatrix.Down, rotationAngle);
            turretForward = Vector3.Transform(turretForward, turretRotateMatrix);
            turretUp = Vector3.Transform(turretUp, turretRotateMatrix);

            Matrix newTurretMatrix = Matrix.CreateWorld(turretPosition, turretForward, turretUp);

            if (Math.Abs(newTurretMatrix.Rotation.Y) < turretMaxRotaion)
            {
                relativeTransform[turretBoneIndex] = newTurretMatrix;
                return true;
            }
            return false;
        }

        public void RotateWheels(float travelDirection)
        {
            for (int i = 0; i < wheelBoneIndex.Length; i++)
            {
                Matrix wheelMatrix = relativeTransform[wheelBoneIndex[i]];
                Vector3 wheelForward = wheelMatrix.Forward;
                Vector3 wheelUp = wheelMatrix.Up;
                Vector3 position = wheelMatrix.Translation;
                
                wheelMatrix = Matrix.CreateFromAxisAngle(wheelMatrix.Right * travelDirection, MathHelper.PiOver4 * 0.15f);
                wheelForward = Vector3.Transform(wheelForward , wheelMatrix);
                wheelUp = Vector3.Transform(wheelUp, wheelMatrix);
                Matrix newFrontWheelMatrix = Matrix.CreateWorld(position, wheelForward, wheelUp);
                relativeTransform[wheelBoneIndex[i]] = newFrontWheelMatrix;
            }

        }

        public void RotateTank(float rotationAngle)
        {
            RotationMatrix *= Matrix.CreateFromAxisAngle(Up, rotationAngle);
        }

        public void Fire() {
            BaseModel bullet = new Bullet(Game.Content.Load<Model>(@"Models\bullet"));
            bullet.Tag = Tag;

            // set bullet position & facing direction
            Matrix canonHeadAbsoluteMatrix = absoluteTranform[canonHeadBoneIndex] * WorldMatrix;
            bullet.WorldMatrix = canonHeadAbsoluteMatrix;
            bullet.physics.MaxSpeed = 1000;
            bullet.physics.Velocity = bullet.Forward * 1000;

            modelManager.Add(bullet);
        }

        public void Destroy() {
            modelManager.Remove(this);
        }

        public Matrix turretMatrix()
        {
            return absoluteTranform[turretBoneIndex] * WorldMatrix;
        }
    }
}
