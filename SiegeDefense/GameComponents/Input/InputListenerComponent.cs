

namespace SiegeDefense {
    public class InputListenerComponent : GameObjectComponent {
        private IInputManager _inputManager;
        protected IInputManager inputManager {
            get {
                if (_inputManager == null) {
                    _inputManager = Game.Services.GetService<IInputManager>();
                }
                return _inputManager;
            }
        }
    }
}
