using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class WireFrameBoxRenderer : _3DRenderer {

        private Vector3[] corners;
        private VertexPositionColor[] vertices = new VertexPositionColor[8];
        private int[] indices = new int[24];
        public WireFrameBoxRenderer(Vector3[] corners, Color color) {
            this.corners = corners;

            for (int i = 0; i < 8; i++) {
                vertices[i].Position = corners[i];
                vertices[i].Color = color;
            }

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 1;
            indices[3] = 2;
            indices[4] = 2;
            indices[5] = 3;
            indices[6] = 3;
            indices[7] = 0;

            indices[8] = 4;
            indices[9] = 5;
            indices[10] = 5;
            indices[11] = 6;
            indices[12] = 6;
            indices[13] = 7;
            indices[14] = 7;
            indices[15] = 4;

            indices[16] = 0;
            indices[17] = 4;
            indices[18] = 1;
            indices[19] = 5;
            indices[20] = 2;
            indices[21] = 6;
            indices[22] = 3;
            indices[23] = 7;
        }

        public void SetColor(Color color) {
            for (int i=0; i<vertices.Length; i++) {
                vertices[i].Color = color;
            }
        }

        public override void Draw(GameTime gameTime) {
            return;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.FogEnabled = false;
            basicEffect.World = baseObject.transformation.WorldMatrix;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 8, indices, 0, 12);
            }

            base.Draw(gameTime);
        }
    }
}
