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
            Vector3 newPosition = Position + moveDirection;
            if (!map.IsInsideMap(newPosition))
                return;

            // angle between map normal & up vector -- calculate map slope
            Vector3 mapNormal = map.GetNormal(newPosition);
            float angle = MathHelper.Clamp(Vector3.Dot(mapNormal, Vector3.Up) / (mapNormal.Length()), -1, 1);
            angle = (float)Math.Acos(angle);
            angle = angle * 180 / MathHelper.Pi;

            if (Math.Abs(angle) > 45) return;

            float newHeight = map.GetHeight(newPosition);
            Vector3 newPositionUpdated = new Vector3(newPosition.X, newHeight, newPosition.Z);

            if (!map.IsAccessibleByFoot(newPositionUpdated)) return;

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
            
            // set bullet position & facing direction
            Matrix canonHeadAbsoluteMatrix = absoluteTranform[canonHeadBoneIndex] * WorldMatrix;

            Vector3 bulletPosition = canonHeadAbsoluteMatrix.Translation;
            Vector3 bulletForward = canonHeadAbsoluteMatrix.Forward;
            Vector3 bulletUp = canonHeadAbsoluteMatrix.Up;

            bullet.WorldMatrix = Matrix.CreateWorld(bulletPosition, bulletForward, bulletUp);

            GamePhysics bulletPhysics = new GamePhysics();
            bulletPhysics.Velocity = bullet.Forward * 1000;
            bullet.AddChild(bulletPhysics);

            modelManager.models.Add(bullet);
        }

        private BasicEffect basicEffect;
        public override void Draw(GameTime gameTime) {

            base.Draw(gameTime);

            BoundingBox canonBaseBounding = OrientedCollisionBox.getBoundingBoxOfModelBone(model, canonBoneIndex);
            Vector3[] boundingCorners = canonBaseBounding.GetCorners();
            Matrix refWorldMatrix = WorldMatrix;
            Vector3.Transform(boundingCorners, ref refWorldMatrix, boundingCorners);

            VertexPositionColor[] vertices = new VertexPositionColor[8];

            for (int i = 0; i < 8; i++) {
                vertices[i].Position = boundingCorners[i];
                vertices[i].Color = Color.DarkOrange;
            }

            int[] indices = new int[24];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 1;
            indices[3] = 2;
            indices[4] = 2;
            indices[5] = 3;
            indices[6] = 3;
            indices[7] = 0;

            indices[8] = 4;
            indices[9] = 5;
            indices[10] = 5;
            indices[11] = 6;
            indices[12] = 6;
            indices[13] = 7;
            indices[14] = 7;
            indices[15] = 4;

            indices[16] = 0;
            indices[17] = 4;
            indices[18] = 1;
            indices[19] = 5;
            indices[20] = 2;
            indices[21] = 6;
            indices[22] = 3;
            indices[23] = 7;

            if (basicEffect == null)
                basicEffect = (BasicEffect)Game.Services.GetService<BasicEffect>().Clone();

            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.FogEnabled = false;
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 8, indices, 0, 12);
            }

            
        }
    }
}
