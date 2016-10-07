using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public abstract class _3DRenderer : Renderer {
        private Camera _camera;
        protected Camera camera {
            get {
                if (_camera == null) {
                    _camera = FindObjects<Camera>()[0];
                }
                return _camera;
            }
        }

        private BasicEffect _basicEffect;
        protected BasicEffect basicEffect {
            get {
                if (_basicEffect == null) {
                    _basicEffect = (BasicEffect)Game.Services.GetService<BasicEffect>().Clone();
                }
                return _basicEffect;
            }
        }

        private Effect _customEffect;
        protected Effect customEffect {
            get {
                if (_customEffect == null) {
                    _customEffect = Game.Services.GetService<Effect>().Clone();
                }
                return _customEffect;
            }
        }
    }
}
