﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;

namespace SiegeDefense {
    public static class Utility {

        public static Rectangle CalculateDrawArea(double x, double y, double width, double height, GraphicsDevice graphics) {

            int screenWidth = graphics.PresentationParameters.BackBufferWidth;
            int screenHeight = graphics.PresentationParameters.BackBufferHeight;
            return new Rectangle((int)(x * screenWidth),
                (int)(y * screenHeight),
                (int)(width * screenWidth),
                (int)(height * screenHeight));
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
            
            float RotationDirection = 1;
            if (Vector3.Dot(left, destination) < 0)
            {
                RotationDirection = -1;
            }

            return (float) Math.Acos(dot) * RotationDirection;
        }

        public static string ToDescription(this Enum value) {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        public static float ToFloat(this object value) {
            return float.Parse(value.ToString());
        }
    }
}
