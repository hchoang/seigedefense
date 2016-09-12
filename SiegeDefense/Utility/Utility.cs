using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SiegeDefense {
    public static class Utility {
        public static object GraphicDevice { get; private set; }

        public static Rectangle CalculateDrawArea(double x, double y, double width, double height, GraphicsDevice graphics) {
            return new Rectangle((int)(x * graphics.DisplayMode.Width),
                (int)(y * graphics.DisplayMode.Height),
                (int)(width * graphics.DisplayMode.Width),
                (int)(height * graphics.DisplayMode.Height));
        }

        public static Rectangle CalculateDrawArea(double x, double y, double width, double height, Rectangle root) {
            return new Rectangle((int)(root.X + x * root.Width),
                (int)(root.Y + y * root.Height),
                (int)(width * root.Width),
                (int)(height * root.Height));
        }

        public static Rectangle CalculateDrawArea(Vector2 position, Vector2 size, GraphicsDevice graphics) {
            return CalculateDrawArea(position.X, position.Y, size.X, size.Y, graphics);
        }

        public static Rectangle CalculateDrawArea(Vector2 position, Vector2 size, Rectangle root) {
            return CalculateDrawArea(position.X, position.Y, size.X, size.Y, root);
        }

        public static float RotationAngleCalculator(Vector3 origin, Vector3 destination, Vector3 left)
        {
            origin.Normalize();
            destination.Normalize();
            float dot = Vector3.Dot(origin, destination);
            
            //if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            //{
            //    return (float) Math.PI;
            //}
            //if (Math.Abs(dot - (1.0f)) < 0.000001f)
            //{
            //    return 0f;
            //}
            float RotationDirection = 1;
            if (Vector3.Dot(left, destination) < 0)
            {
                RotationDirection = -1;
            }

            float cosForward = dot / (origin.Length() * destination.Length());

            return (float) Math.Acos(cosForward) * RotationDirection;
        }
    }
}
