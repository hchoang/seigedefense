using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public class OnlandVehicle : _3DGameObject
    {
        public virtual int HP { get; set; }
        public virtual Model vehicleModel { get; set; }
        public virtual void Destroy() { }
        public virtual void Fire() { }
        public new OnlandVehiclePhysics physics { get; set; }
        public HPRenderer hpRenderer { get; set; }

        public override void Update(GameTime gameTime) {
            hpRenderer.currentHP = HP;

            base.Update(gameTime);
        }

        public virtual void RotateVehicle(float rotationAngle) {
            transformation.RotationMatrix *= Matrix.CreateFromAxisAngle(transformation.Up, rotationAngle);
        }

        public virtual bool Moveable(Vector3 testPosition) {
            if (!map.IsAccessibleByFoot(testPosition))
                return false;

            // collision check with other tanks
            Vector3 oldPosition = transformation.Position;
            transformation.Position = testPosition;

            foreach (OnlandVehicle tank in FindObjectsInPartition<OnlandVehicle>()) {
                if (tank == this) continue;
                if (collider.SphereIntersect(tank.collider)) {
                    transformation.Position = oldPosition;
                    return false;
                }
            }

            transformation.Position = oldPosition;
            return true;
        }

        // return true: vehicle moved successfully
        public virtual bool Move(Vector3 moveDirection) {
            Vector3 newPosition = transformation.Position + moveDirection;
            if (!Moveable(newPosition))
                return false;

            newPosition.Y = map.GetHeight(newPosition);
            transformation.Position = newPosition;
            Vector3 mapNormal = map.GetNormal(transformation.Position);
            transformation.Up = mapNormal;

            return true;
        }


        // return true: vehicle destroyed
        public virtual bool Damaged(int damage) {
            if (this.HP <= 0) {
                return false;
            }
            this.HP -= damage;
            if (this.HP <= 0) {
                this.Destroy();
                return true;
            }

            return false;
        }

        public virtual void RotateWheels(float rotateAngle) { }
    }
}
