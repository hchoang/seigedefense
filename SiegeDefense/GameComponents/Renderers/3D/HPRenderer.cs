using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class HPRenderer : _3DRenderer {

        protected Texture2D hpTexture { get; set; }
        public Vector3 rotateAxis {
            get {
                return baseObject.transformation.Up;
            }
        }
        public float currentHP { get; set; } = 100;
        public float maxHP { get; set; } = 100;
        private float hpRatio = 100;
        private float hpBarLength = 30;
        protected Vector3 positionOffset { get; set; } = Vector3.Zero;

        protected VertexPositionTexture[] vertices = new VertexPositionTexture[6];

        public HPRenderer(Vector3 positionOffset) {
            this.hpTexture = Game.Content.Load<Texture2D>(@"Sprites\WhiteBar");
            this.positionOffset = positionOffset;

            customEffect.CurrentTechnique = customEffect.Techniques["Billboard"];

            vertices[0] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            vertices[1] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 0));
            vertices[2] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));

            vertices[3] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            vertices[4] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));
            vertices[5] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 1));
        }

        public override void Update(GameTime gameTime) {
            this.transformation.Position = baseObject.transformation.Position + positionOffset;

            hpRatio = currentHP / maxHP;
            this.transformation.ScaleMatrix = Matrix.CreateScale(hpBarLength * hpRatio, 1, 1);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            if (!GameObject.isHPBarDisplay) {
                return;
            }

            customEffect.Parameters["World"].SetValue(transformation.WorldMatrix);
            customEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            customEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            customEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            customEffect.Parameters["RotateAxis"].SetValue(rotateAxis);
            customEffect.Parameters["BillboardTexture"].SetValue(hpTexture);
            customEffect.Parameters["BillboardHeight"].SetValue(transformation.ScaleMatrix.Scale.Y);
            customEffect.Parameters["BillboardSide"].SetValue(transformation.ScaleMatrix.Scale.X);
            customEffect.Parameters["MaskColor"].SetValue(new Vector4(1-hpRatio, hpRatio, 0, 0));

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
