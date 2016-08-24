﻿using Microsoft.Xna.Framework;
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
        protected int[] wheelBoneIndex;

        public Tank(Model model) : base(model)
        {
            wheelBoneIndex = new int[4];

            wheelBoneIndex[0] = model.Bones["l_front_wheel_geo"].Index;
            wheelBoneIndex[1] = model.Bones["r_front_wheel_geo"].Index;
            wheelBoneIndex[2] = model.Bones["l_back_wheel_geo"].Index;
            wheelBoneIndex[3] = model.Bones["r_back_wheel_geo"].Index;

            turretBoneIndex = model.Bones["turret_geo"].Index;  
            canonBoneIndex = model.Bones["canon_geo"].Index;       
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Move(Vector3 moveDirection) {
            Vector3 newPosition = Position + moveDirection;
            if (!map.Moveable(newPosition))
                return;

            Vector3 mapNormal = map.GetNormal(newPosition);
            // angle between map normal & up vector -- calculate map slope
            float angle = MathHelper.Clamp(Vector3.Dot(mapNormal, Vector3.Up) / (mapNormal.Length()), -1, 1);
            angle = (float)Math.Acos(angle);
            angle = angle * 180 / MathHelper.Pi;

            if (Math.Abs(angle) > 45) return;

            float newHeight = map.GetHeight(newPosition);
            Vector3 newPositionUpdated = new Vector3(newPosition.X, newHeight, newPosition.Z);

            float travelDistance = (newPositionUpdated - Position).Length();

            RotateWheels(travelDistance);
            Position = new Vector3(newPosition.X, newHeight, newPosition.Z);
            Up = mapNormal;
        }

        public void RotateCanon(float rotationAngle)
        {
            Matrix canonMatrix = relativeTransform[canonBoneIndex];
            Vector3 canonForward = canonMatrix.Forward;
            Vector3 canonPosition = canonMatrix.Translation;
            Vector3 canonUp = canonMatrix.Up;

            Matrix canonRotateMatrix = Matrix.CreateFromAxisAngle(canonMatrix.Left, -rotationAngle);
            canonForward = Vector3.Transform(canonForward, canonRotateMatrix);
            canonForward.Normalize();
            if (-0.5f < canonForward.Y && canonForward.Y < 0.2f)
            {
                canonUp = Vector3.Transform(canonUp, canonRotateMatrix);
                relativeTransform[canonBoneIndex] = Matrix.CreateWorld(canonPosition, canonForward, canonUp);
            }
        }

        public void RotateTurret(float rotationAngle)
        {
            Matrix turretMatrix = relativeTransform[turretBoneIndex];
            Vector3 turretForward = turretMatrix.Forward;
            Vector3 turretPosition = turretMatrix.Translation;
            Vector3 turretUp = turretMatrix.Up;

            Matrix turretRotateMatrix = Matrix.CreateFromAxisAngle(turretMatrix.Up, -rotationAngle);
            turretForward = Vector3.Transform(turretForward, turretRotateMatrix);
            turretUp = Vector3.Transform(turretUp, turretRotateMatrix);

            Matrix newTurretMatrix = Matrix.CreateWorld(turretPosition, turretForward, turretUp);
            if (Math.Abs(newTurretMatrix.Rotation.Y) < 0.2f)
            {
                relativeTransform[turretBoneIndex] = newTurretMatrix;
            }
        }

        public void RotateWheels(float travelDistance)
        {
            for (int i = 0; i < wheelBoneIndex.Length; i++)
            {
                Matrix wheelMatrix = relativeTransform[wheelBoneIndex[i]];
                Vector3 wheelForward = wheelMatrix.Forward;
                Vector3 wheelUp = wheelMatrix.Up;
                Vector3 position = wheelMatrix.Translation;

                wheelMatrix = Matrix.CreateFromAxisAngle(wheelMatrix.Right, MathHelper.PiOver4 * 0.25f);
                wheelForward = Vector3.Transform(wheelForward, wheelMatrix);
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
            BaseModel bullet = new Bullet(Game.Content.Load<Model>(@"Models\bullet"), Vector3.Zero);

            // calculate bullet scale to appropriate size
            OrientedCollisionBox bulletCollisionBox = bullet.FindComponent<OrientedCollisionBox>();
            OrientedCollisionBox tankCollisionBox = FindComponent<OrientedCollisionBox>();
            Vector3 bulletBaseSize = bulletCollisionBox.baseBoundingBox.Max - bulletCollisionBox.baseBoundingBox.Min;
            Vector3 tankActualSize = (tankCollisionBox.baseBoundingBox.Max - tankCollisionBox.baseBoundingBox.Min) * ScaleMatrix.Scale;
            Vector3 expectedSizeRatio = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 bulletScale = expectedSizeRatio * tankActualSize / bulletBaseSize;
            
            // set bullet position & facing direction
            Matrix canonAbosuluteMatrix = absoluteTranform[canonBoneIndex];
            Matrix canonWorldMatrix = canonAbosuluteMatrix * WorldMatrix;
            Vector3 canonPosition = canonWorldMatrix.Translation;
            Vector3 canonForward = canonWorldMatrix.Forward;
            Vector3 canonUp = canonWorldMatrix.Up;

            bullet.WorldMatrix = Matrix.CreateWorld(canonPosition, canonForward, canonUp);
            bullet.ScaleMatrix = Matrix.CreateScale(bulletScale);

            GamePhysics bulletPhysics = new GamePhysics();
            bulletPhysics.Velocity = -bullet.Forward * 1000;
            bullet.AddChild(bulletPhysics);

            modelManager.models.Add(bullet);
        }
    }
}
