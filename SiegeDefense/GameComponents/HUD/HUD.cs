using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class HUD : GameObjectComponent {
        private IInputManager _inputManager;
        public IInputManager inputManager {
            get {
                if (_inputManager == null) {
                    _inputManager = Game.Services.GetService<IInputManager>();
                }
                return _inputManager;
            }
        }

        public Action<HUD> onClick { get; set; }

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
