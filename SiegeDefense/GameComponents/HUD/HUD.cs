using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class HUD : InputListenerComponent {

        public Action<HUD> onClick { get; set; }
        public Action<HUD> onBlur { get; set; }

        public _2DRenderer renderer { get; set; }

        public override void Update(GameTime gameTime) {
            if (renderer == null) {
                return;
            }

            float x = inputManager.GetValue(GameInput.PointerX);
            float y = inputManager.GetValue(GameInput.PointerY);

            if (renderer.GetDrawArea().Contains(x, y)) {
                if (inputManager.isReleased(GameInput.Fire)) {
                    onClick?.Invoke(this);
                }
            } else {
                if (inputManager.isReleased(GameInput.Fire)) {
                    onBlur?.Invoke(this);
                }
            }

            base.Update(gameTime);
        }

        public HUD(_2DRenderer renderer) {
            this.renderer = renderer;
            AddComponent(renderer);
        }

        public HUD() { }
    }
}
