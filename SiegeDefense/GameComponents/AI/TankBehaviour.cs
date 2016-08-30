
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Models;
using System;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.AI {
    public static class TankBehaviour {

        //private static Random RNG = new Random();

        public static Vector3 ChaseTargetBehaviour(Matrix currentTransform, Matrix targetTransform, float minDistance = 50) {
            Vector3 currentPosition = currentTransform.Translation;
            Vector3 targetPosition = targetTransform.Translation;
            if ((targetPosition - currentPosition).Length() < minDistance)
                return Vector3.Zero;
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
        
        public static Vector3 MovingOnLandBehaviour(Matrix currentTransform, Map map, float scanRadius = 100) {
            Vector3 originalScanVector = Vector3.Normalize(currentTransform.Forward) * scanRadius;
            Vector3 originalSeePoint = currentTransform.Translation + originalScanVector;

            // do not change steering force if can move
            if (map.IsAccessibleByFoot(originalSeePoint))
                return Vector3.Zero;

            // scan around to find moveable position
            float rotateAngle = 0;
            float rotateStep = MathHelper.PiOver4 / 9;

            while(rotateAngle < MathHelper.TwoPi) {
                rotateAngle += rotateStep;

                Matrix rotateMatrix = Matrix.CreateRotationY(rotateAngle);
                Vector3 scanPoint = currentTransform.Translation + Vector3.Transform(originalScanVector, rotateMatrix);
                scanPoint.Y = map.GetHeight(scanPoint);

                if (map.IsAccessibleByFoot(scanPoint)) {
                    return scanPoint - originalSeePoint;
                }

            }
            
            return Vector3.Zero;
        }
    }
}
