﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class Tank : _3DGameObject
    {
        public int HP { get; set; }
        public float MoveSpeed { get; set; } = 50.0f;
        public float RotateSpeed { get; set; } = 2.0f;
        public float TurretRotateSpeed { get; set; } = 0.05f;
        public float CanonRotateSpeed { get; set; } = 0.05f;

        public new TankRenderer renderer { get; set; }

        public Tank(ModelType modelType)
        {
            Model tankModel = Game.Content.Load<Model>(modelType.ToDescription());
            
            renderer = new TankRenderer(tankModel);
            AddComponent(renderer);

            collider = new Collider(tankModel);
            AddComponent(collider);

            HP = 100;   
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool Moveable(Vector3 testPosition) {
            if (!map.IsAccessibleByFoot(testPosition))
                return false;

            // collision check with other tanks
            Vector3 oldPosition = transformation.Position;
            transformation.Position = testPosition;

            foreach (Tank tank in FindObjects<Tank>()) {
                if (tank == this) continue;
                if (collider.SphereIntersect(tank.collider)) {
                    transformation.Position = oldPosition;
                    return false;
                }
            }

            transformation.Position = oldPosition;
            return true;
        }

        public bool Move(Vector3 moveDirection) {
            Vector3 newPosition = transformation.Position + moveDirection;
            if (!Moveable(newPosition))
                return false;

            newPosition.Y = map.GetHeight(newPosition);
            transformation.Position = newPosition;
            Vector3 mapNormal = map.GetNormal(transformation.Position);
            transformation.Up = mapNormal;

            return true;
        }

        public void RotateTank(float rotationAngle)
        {
            transformation.RotationMatrix *= Matrix.CreateFromAxisAngle(transformation.Up, rotationAngle);
        }

        public virtual void Fire() {
            TankAmmo bullet = new TankAmmo(ModelType.BULLET1, this);
            bullet.Tag = this.Tag + "Bullet";

            // set bullet position & facing direction
            Matrix canonHeadAbsoluteMatrix = renderer.GetCanonHeadAbsolouteMatrix() * transformation.WorldMatrix;
            bullet.transformation.WorldMatrix = canonHeadAbsoluteMatrix;
            bullet.physics.MaxSpeed = 500;
            bullet.physics.Velocity = bullet.transformation.Forward * 1000;

            Game.Components.Add(bullet);

            // play sound
            SoundBankManager soundManager = Game.Services.GetService<SoundBankManager>();
            soundManager.PlaySound(SoundType.TankFire);
        }

        public void Destroy() {
            Game.Components.Remove(this);

            // Drop power-up
            if (Tag.Equals("Enemy")) {
                Item item = new HPRestoreItem();
                item.transformation.Position = transformation.Position;
                item.transformation.ScaleMatrix = Matrix.CreateScale(10);

                Game.Components.Add(item);
            }
        }

        public void Damaged(TankAmmo bullet)
        {
            this.HP -= bullet.damage;
            if (this.HP <= 0)
            {
                if (bullet.owner.Tag.Equals("Player"))
                {
                    Game.Services.GetService<GameManager>().Point += 10;
                }
                this.Destroy();
            }
        }
    }
}
