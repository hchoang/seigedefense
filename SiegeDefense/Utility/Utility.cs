using Microsoft.Xna.Framework;

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
    }
}
