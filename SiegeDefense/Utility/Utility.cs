using Microsoft.Xna.Framework;
using System;

namespace SiegeDefense {
    public static class Utility {
        public static Rectangle CalculateDrawArea(double x, double y, double width, double height, GraphicsDeviceManager graphics) {
            return new Rectangle((int)(x * graphics.PreferredBackBufferWidth),
                (int)(y * graphics.PreferredBackBufferHeight),
                (int)(width * graphics.PreferredBackBufferWidth),
                (int)(height * graphics.PreferredBackBufferHeight));
        }

        public static Rectangle CalculateDrawArea(double x, double y, double width, double height, Rectangle root) {
            return new Rectangle((int)(root.X + x * root.Width),
                (int)(root.Y + y * root.Height),
                (int)(width * root.Width),
                (int)(height * root.Height));
        }

        public static Rectangle CalculateDrawArea(Vector2 position, Vector2 size, GraphicsDeviceManager graphics) {
            return CalculateDrawArea(position.X, position.Y, size.X, size.Y, graphics);
        }

        public static Rectangle CalculateDrawArea(Vector2 position, Vector2 size, Rectangle root) {
            return CalculateDrawArea(position.X, position.Y, size.X, size.Y, root);
        }

        public static float RotationAngleCalculator(Vector3 origin, Vector3 destination)
        {
            float dot = Vector3.Dot(origin, destination);
            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                return (float) Math.PI;
            }
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                return 0f;
            }


            return (float) Math.Acos(dot / (origin.Length() * destination.Length()));
        }
    }
}
