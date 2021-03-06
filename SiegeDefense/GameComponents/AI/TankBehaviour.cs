﻿
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SiegeDefense {
    public static class TankBehaviour {

        //private static Random RNG = new Random();
        //public const int nZone = 36;
        //public static void ChaseTargetContext(Matrix currentTransform, Matrix targetTransform, ref Vector3[] dangerZone, ref Vector3[] interestZone) {
        //    float rotateUnit = MathHelper.TwoPi / nZone;
        //    for (int i = 0; i < nZone; i++) {
        //        Matrix rotateMatrix = Matrix.CreateRotationY()
        //    }
        //}

        public static Vector3 ChaseTargetBehaviour(Matrix currentTransform, Matrix targetTransform, float minDistance = 80) {
            Vector3 currentPosition = currentTransform.Translation;
            currentPosition.Y = 0;
            Vector3 targetPosition = targetTransform.Translation;
            targetPosition.Y = 0;
            if ((targetPosition - currentPosition).Length() < minDistance)
                return currentTransform.Forward;
            return targetPosition - currentPosition;
        }

        public static Vector3 WanderingBehaviour(Matrix currentTransform, int wanderingRadius = 100) {
            // create random target around object
            Random RNG = new Random();
            int randX = RNG.Next(-wanderingRadius, wanderingRadius);
            int randZ = RNG.Next(-wanderingRadius, wanderingRadius);
            Vector3 randomTarget = new Vector3(randX, 0, randZ);
            randomTarget += currentTransform.Translation;
            return ChaseTargetBehaviour(currentTransform, Matrix.CreateTranslation(randomTarget), 0);
        }
        
        public static Vector3 AdvoidObstacleBehaviour(OnlandVehicle tank, Vector3 currentSteering, Map map, float scanRadius = 50) {
            Vector3 originalScanVector = Vector3.Normalize(currentSteering) * scanRadius;
            //Vector3 originalSeePoint = currentTransform.Translation + originalScanVector;

            // scan around to find moveable position
            float rotateAngle = 0;
            float rotateStep = MathHelper.PiOver4;

            List<float> availableAngles = new List<float>();
            Matrix rotateMatrix;
            while (rotateAngle < MathHelper.TwoPi) {

                rotateMatrix = Matrix.CreateRotationY(rotateAngle);
                Vector3 scanPoint = tank.transformation.Position + Vector3.Transform(originalScanVector, rotateMatrix);

                if (tank.Moveable(scanPoint)) {
                    availableAngles.Add(rotateAngle);
                    return Vector3.Transform(currentSteering, rotateMatrix);
                }

                rotateAngle += rotateStep;
            }

            if (availableAngles.Count == 0) {
                return tank.transformation.WorldMatrix.Backward;
            }

            availableAngles.Sort(new AngleComparer());
            rotateMatrix = Matrix.CreateRotationY(availableAngles[0]);
            return Vector3.Transform(currentSteering, rotateMatrix);
        }
    }

    public class AngleComparer : Comparer<float>{
        public override int Compare(float x, float y) {
            float newX = Math.Abs(x - MathHelper.Pi);
            float newY = Math.Abs(y - MathHelper.Pi);
            return newY.CompareTo(newX);
        }
    }
}
