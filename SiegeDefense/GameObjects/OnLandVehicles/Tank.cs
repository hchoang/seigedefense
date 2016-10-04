using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class Tank : OnlandVehicle {
        public float TurretRotateSpeed { get; set; } = 0.05f;
        public float CanonRotateSpeed { get; set; } = 0.05f;

        public new TankRenderer renderer { get; set; }

        public override void RotateWheels(float rotateAngle) {
            this.renderer.RotateWheels(rotateAngle);
        }

        public override void Fire() {
            TankBullet bullet = new TankBullet(ModelType.BULLET1, this);
            bullet.Tag = this.Tag + "Bullet";

            // set bullet position & facing direction
            bullet.transformation.WorldMatrix = renderer.GetCanonHeadWorldMatrix();
            bullet.transformation.ScaleMatrix = Matrix.Identity;
            bullet.AddToGameWorld();
            //Game.Components.Add(bullet);

            // play sound
            SoundBankManager soundManager = Game.Services.GetService<SoundBankManager>();
            soundManager.PlaySound(SoundType.TankFire);
        }

        public override void Destroy() {
            RemoveFromGameWorld();

            // Explosion
            Explosion explosion = new Explosion();
            explosion.transformation.Position = this.transformation.Position;
            explosion.transformation.ScaleMatrix = Matrix.CreateScale(collider.baseBoundingBox.Max - collider.baseBoundingBox.Min);
            Game.Components.Add(explosion);

            // Drop power-up
            if (Tag.Equals("Enemy")) {
                Item item = new HPRestoreItem();
                item.transformation.Position = transformation.Position;
                item.transformation.ScaleMatrix = Matrix.CreateScale(10);

                item.AddToGameWorld();
            }
        }
    }
}
