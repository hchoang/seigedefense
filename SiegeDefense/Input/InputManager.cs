using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SiegeDefense.GameObjects;

namespace SiegeDefense.Input {
    public class InputManager : GameObject, IInputManager {
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private MouseState currentMouseState;
        private MouseState previousMouseState;

        private enum MouseButton {
            LeftButton,
            RightButton,
            MiddleButton,
            XButton1,
            XButton2
        }

        public override void Update(GameTime gameTime) {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public float GetValue(GameInput input, bool isCurrent = true) {

            // Logical to physical input mapping
            if (input == GameInput.Up)
                return GetValue(Keys.Up, isCurrent);

            if (input == GameInput.Down)
                return GetValue(Keys.Down, isCurrent);

            if (input == GameInput.Left)
                return GetValue(Keys.Left, isCurrent);

            if (input == GameInput.Right)
                return GetValue(Keys.Right, isCurrent);

            if (input == GameInput.Zoom)
                return GetMouseScroll(false) - GetMouseScroll(true);

            return 0;
        }

        public bool isPressing(GameInput input) {
            return GetValue(input) != 0;
        }

        public bool isTriggered(GameInput input) {
            return GetValue(input, false) == 0 && GetValue(input) != 0;
        }

        public bool isReleased(GameInput input) {
            return GetValue(input, false) != 0 && GetValue(input) == 0;
        }

        private float GetValue(Keys key, bool isCurrent) {
            KeyboardState kbs = isCurrent ? currentKeyboardState : previousKeyboardState;

            return kbs.IsKeyDown(key) ? 1 : 0;
        }

        private float GetValue(MouseButton button, bool isCurrent) {
            MouseState ms = isCurrent ? currentMouseState : previousMouseState;
            ButtonState state = (ButtonState)typeof(MouseState).GetProperty(button.ToString()).GetValue(ms);
            return state == ButtonState.Pressed ? 1 : 0;
        }

        private float GetMouseScroll(bool isCurrent) {
            MouseState ms = isCurrent ? currentMouseState : previousMouseState;
            return ms.ScrollWheelValue;
        }

        private Vector2 getMousePosition(bool isCurrent) {
            MouseState ms = isCurrent ? currentMouseState : previousMouseState;
            return ms.Position.ToVector2();
        }
    }
}
