using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class BillboardRenderer : _3DRenderer {
        public Texture2D texture { get; set; }
        public Vector3 rotateAxis { get; set; } = Vector3.Up;
        protected VertexPositionTexture[] vertices = new VertexPositionTexture[6];

        public BillboardRenderer(Texture2D texture) {
            this.texture = texture;
            customEffect.CurrentTechnique = customEffect.Techniques["Billboard"];

            vertices[0] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            vertices[1] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 0));
            vertices[2] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));

            vertices[3] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            vertices[4] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));
            vertices[5] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 1));
        }

        public override void Draw(GameTime gameTime) {
            customEffect.Parameters["World"].SetValue(baseObject.transformation.WorldMatrix);
            customEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            customEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            customEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            customEffect.Parameters["RotateAxis"].SetValue(rotateAxis);
            customEffect.Parameters["BillboardTexture"].SetValue(texture);
            customEffect.Parameters["BillboardHeight"].SetValue(baseObject.transformation.ScaleMatrix.Scale.Y);
            customEffect.Parameters["BillboardSide"].SetValue(baseObject.transformation.ScaleMatrix.Scale.X);

            BlendState oldBS = GraphicsDevice.BlendState;
            DepthStencilState oldDS = GraphicsDevice.DepthStencilState;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach (EffectPass pass in customEffect.CurrentTechnique.Passes) {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
            }

            GraphicsDevice.BlendState = oldBS;
            GraphicsDevice.DepthStencilState = oldDS;

            base.Draw(gameTime);
        }
    }
}
