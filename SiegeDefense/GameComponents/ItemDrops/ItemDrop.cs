using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameComponents.Models;
using SiegeDefense.GameComponents.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.ItemDrops {
    public abstract class ItemDrop : BaseModel {
        protected Texture2D texture { get; set; }
        public Vector3 rotateAxis { get; set; } = Vector3.Up;
        protected Effect billboardEffect;
        protected VertexPositionTexture[] vertices = new VertexPositionTexture[6];
        private Camera _camera;
        protected Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }

        public ItemDrop() {
            billboardEffect = Game.Services.GetService<Effect>().Clone();
            billboardEffect.CurrentTechnique = billboardEffect.Techniques["Billboard"];

            vertices[0] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            vertices[1] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 0));
            vertices[2] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));

            vertices[3] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            vertices[4] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));
            vertices[5] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 1));

            collisionBox = new OrientedCollisionBox(new BoundingBox(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 1, 0.5f)));
            AddChild(collisionBox);
        }

        public override void Draw(GameTime gameTime) {
            billboardEffect.Parameters["World"].SetValue(WorldMatrix);
            billboardEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            billboardEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            billboardEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            billboardEffect.Parameters["RotateAxis"].SetValue(rotateAxis);
            billboardEffect.Parameters["BillboardTexture"].SetValue(texture);
            billboardEffect.Parameters["BillboardHeight"].SetValue(ScaleMatrix.Scale.Y);
            billboardEffect.Parameters["BillboardSide"].SetValue(ScaleMatrix.Scale.X);

            BlendState oldBS = GraphicsDevice.BlendState;
            DepthStencilState oldDS = GraphicsDevice.DepthStencilState;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach (EffectPass pass in billboardEffect.CurrentTechnique.Passes) {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
            }

            GraphicsDevice.BlendState = oldBS;
            GraphicsDevice.DepthStencilState = oldDS;

            collisionBox.Draw(gameTime);
            //base.Draw(gameTime);
        }
    }
}
