using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public static void GetSubTextures(Texture2D largeTexture, int X, int Y, int offsetX, int offsetY, int nX, int nY, 
            GraphicsDevice graphicsDevice, out List<Texture2D> result) {

            Point textureSize = new Point(largeTexture.Width / nX, largeTexture.Height / nY);

            result = new List<Texture2D>();
            for (int x = 0; x < X; x++) {
                for (int y = 0; y < Y; y++) {

                    Point startPoint = new Point(largeTexture.Width * (offsetX + x) / nX, largeTexture.Height * (offsetY + y) / nY);
                    Rectangle rect = new Rectangle(startPoint, textureSize);

                    Texture2D smallTexture = new Texture2D(graphicsDevice, rect.Width, rect.Height);
                    Color[] smallTextureData = new Color[rect.Width * rect.Height];
                    largeTexture.GetData(0, rect, smallTextureData, 0, smallTextureData.Length);
                    smallTexture.SetData(smallTextureData);

                    result.Add(smallTexture);
                }
            }
        }
    }
}
