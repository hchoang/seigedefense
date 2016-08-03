using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SiegeDefense.Input {
    public class InputManager {
        private Dictionary<GameInput, Keys> inputToKeyboardMap = new Dictionary<GameInput, Keys>();
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;

        public InputManager() {
            inputToKeyboardMap.Add(GameInput.Up, Keys.Up);
            inputToKeyboardMap.Add(GameInput.Down, Keys.Down);
            inputToKeyboardMap.Add(GameInput.Left, Keys.Left);
            inputToKeyboardMap.Add(GameInput.Right, Keys.Right);
        }

        public bool isPressing(GameInput input) {
            Keys key;
            bool isKeyMapped = inputToKeyboardMap.TryGetValue(input, out key);

            if (!isKeyMapped)
                return false;

            return currentKeyboardState.IsKeyDown(key);
        }

        public void Update() {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }
    }
}
