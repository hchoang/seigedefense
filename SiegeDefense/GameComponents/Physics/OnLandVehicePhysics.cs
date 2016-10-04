using Microsoft.Xna.Framework;
using System;

namespace SiegeDefense {
    public class OnlandVehiclePhysics : GamePhysics {
        public float Mass { get; set; } = 1;

        public float ForwardForce { get; set; } = 0;
        public float Acceleration { get; set; } = 0;
        public float MoveSpeed { get; set; } = 0;
        public float MaxSpeed { get; set; } = 60f;
        public float MinSpeed { get; set; } = -50f;
        public float RotateForce { get; set; } = 0;
        public float RotateSpeed { get; set; } = 0f; // in radian/s
        public float RotateAcceleration { get; set; } = 0f;
        public float MaxRotateSpeed { get; set; } = 1.2f; // in radian/s
        public float Inertia { get; set; } = 0.75f;
        public float RotateInertia { get; set; } = 0.2f;
        public Vector3 ImpactForce { get; set; } = Vector3.Zero;
        public Vector3 ImpactAcceleration { get; set; } = Vector3.Zero;
        public Vector3 ImpactVelocity { get; set; } = Vector3.Zero;
        public float ImpactRecovery { get; set; } = 1f;

        protected OnlandVehicle vehicle {
            get {
                return (OnlandVehicle)baseObject;
            }
        }

        public override void Update(GameTime gameTime) {

            Acceleration = ForwardForce / Mass;
            RotateAcceleration = RotateForce / Mass;

            MoveSpeed += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            RotateSpeed += RotateAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (MoveSpeed > MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds) {
                MoveSpeed = MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (MoveSpeed < MinSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds) {
                MoveSpeed = MinSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Math.Abs(RotateSpeed) > MaxRotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds) {
                int i = RotateSpeed < 0 ? -1 : 1;
                RotateSpeed = MaxRotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * i;
            }

            // apply inertia
            if (Math.Abs(MoveSpeed) < Inertia * Mass * (float)gameTime.ElapsedGameTime.TotalSeconds) {
                MoveSpeed = 0;
            } else {
                if (MoveSpeed > 0) {
                    MoveSpeed -= Inertia * Mass * (float)gameTime.ElapsedGameTime.TotalSeconds;
                } else {
                    MoveSpeed += Inertia * Mass * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if (RotateForce == 0) {
                if (RotateSpeed < RotateInertia * Mass * (float)gameTime.ElapsedGameTime.TotalSeconds) {
                    RotateSpeed = 0;
                } else {
                    RotateSpeed -= RotateInertia * Mass * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            ImpactAcceleration = ImpactForce / Mass;
            if (ImpactForce != Vector3.Zero) {
                ImpactForce = Vector3.Zero;
            }
            ImpactVelocity += ImpactAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool moved = vehicle.Move(MoveSpeed * Vector3.Normalize(vehicle.transformation.Forward) + ImpactVelocity);
            vehicle.RotateVehicle(RotateSpeed);

            if (moved) {
                vehicle.RotateWheels(-MoveSpeed);
            } else {
                MoveSpeed = 0;
            }

            if (ImpactVelocity.Length() < ImpactRecovery * (float)gameTime.ElapsedGameTime.TotalSeconds) {
                ImpactVelocity = Vector3.Zero;
                ImpactForce = Vector3.Zero;
            } else {
                ImpactVelocity -= Vector3.Normalize(ImpactVelocity) * ImpactRecovery * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }
    }
}
