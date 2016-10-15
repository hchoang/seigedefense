using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class ExplosiveTruck : OnlandVehicle {

        public override void Fire() {
            Destroy();
        }

        bool destroyed = false;
        public override void Destroy() {

            if (destroyed) {
                return;
            }
            destroyed = true;

            // Explosion
            Explosion explosion = new Explosion();
            explosion.transformation.Position = this.transformation.Position;
            explosion.transformation.ScaleMatrix = Matrix.CreateScale( (collider.baseBoundingBox.Max - collider.baseBoundingBox.Min) * 10 );
            Game.Components.Add(explosion);

            // Push nearby vehicles
            foreach (OnlandVehicle vehicle in FindObjects<OnlandVehicle>()) {
                if (vehicle == this) {
                    continue;
                }
                Vector3 impactForce = vehicle.transformation.Position - this.transformation.Position;
                float distance = impactForce.Length();

                if (distance > 200) {
                    continue;
                }

                float forceMagnitude = MathHelper.Clamp(10000 / distance, 0, 100);
                impactForce = Vector3.Normalize(impactForce) * forceMagnitude;
                vehicle.physics.ImpactForce = impactForce;
                vehicle.Damaged((int)forceMagnitude);
            }

            RemoveFromGameWorld();
        }
    }
}
