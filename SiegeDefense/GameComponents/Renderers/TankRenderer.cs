using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SiegeDefense {
    public class TankRenderer : ModelRenderer {
        protected int[] wheelBoneIndex;
        protected int turretBoneIndex;
        protected int canonBoneIndex;
        protected int canonHeadBoneIndex;

        public TankRenderer(Model model) : base(model) {
            wheelBoneIndex = new int[4];

            wheelBoneIndex[0] = model.Bones["l_front_wheel_geo"].Index;
            wheelBoneIndex[1] = model.Bones["r_front_wheel_geo"].Index;
            wheelBoneIndex[2] = model.Bones["l_back_wheel_geo"].Index;
            wheelBoneIndex[3] = model.Bones["r_back_wheel_geo"].Index;

            turretBoneIndex = model.Bones["turret_geo"].Index;
            canonBoneIndex = model.Bones["canon_geo"].Index;
            canonHeadBoneIndex = model.Bones["canon_head_geo"].Index;
        }

        public void RotateCanon(float rotationAngle) {
            Matrix canonMatrix = relativeTransform[canonBoneIndex];
            Vector3 canonForward = canonMatrix.Forward;
            Vector3 canonPosition = canonMatrix.Translation;
            Vector3 canonUp = canonMatrix.Up;

            Matrix canonRotateMatrix = Matrix.CreateFromAxisAngle(canonMatrix.Left, rotationAngle);
            canonForward = Vector3.Transform(canonForward, canonRotateMatrix);
            canonForward.Normalize();
            if (-0.2f < canonForward.Y && canonForward.Y < 0.5f) {
                canonUp = Vector3.Transform(canonUp, canonRotateMatrix);
                relativeTransform[canonBoneIndex] = Matrix.CreateWorld(canonPosition, canonForward, canonUp);
            }
        }

        public bool RotateTurret(float rotationAngle) {
            Matrix turretMatrix = relativeTransform[turretBoneIndex];
            Vector3 turretForward = turretMatrix.Forward;
            Vector3 turretPosition = turretMatrix.Translation;
            Vector3 turretUp = turretMatrix.Up;

            Matrix turretRotateMatrix = Matrix.CreateFromAxisAngle(turretMatrix.Down, rotationAngle);
            turretForward = Vector3.Transform(turretForward, turretRotateMatrix);
            turretUp = Vector3.Transform(turretUp, turretRotateMatrix);

            Matrix newTurretMatrix = Matrix.CreateWorld(turretPosition, turretForward, turretUp);

            if (Math.Abs(newTurretMatrix.Rotation.Y) < 0.2f) {
                relativeTransform[turretBoneIndex] = newTurretMatrix;
                return true;
            }
            return false;
        }

        public void RotateWheels(float travelDirection) {
            for (int i = 0; i < wheelBoneIndex.Length; i++) {
                Matrix wheelMatrix = relativeTransform[wheelBoneIndex[i]];
                Vector3 wheelForward = wheelMatrix.Forward;
                Vector3 wheelUp = wheelMatrix.Up;
                Vector3 position = wheelMatrix.Translation;

                wheelMatrix = Matrix.CreateFromAxisAngle(wheelMatrix.Right * travelDirection, MathHelper.PiOver4 * 0.15f);
                wheelForward = Vector3.Transform(wheelForward, wheelMatrix);
                wheelUp = Vector3.Transform(wheelUp, wheelMatrix);
                Matrix newFrontWheelMatrix = Matrix.CreateWorld(position, wheelForward, wheelUp);
                relativeTransform[wheelBoneIndex[i]] = newFrontWheelMatrix;
            }
        }

        public Matrix GetCanonHeadAbsolouteMatrix() {
            return absoluteTranform[canonHeadBoneIndex];
        }
    }
}
