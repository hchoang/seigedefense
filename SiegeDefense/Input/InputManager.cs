using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SiegeDefense.Input {
    public class InputManager {

        private enum State {
            Current,
            Previous
        }

        private Dictionary<GameInput, Keys> inputToKeyboardMap = new Dictionary<GameInput, Keys>();
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;

        private Dictionary<GameInput, string> inputToMouseButtonMap = new Dictionary<GameInput, string>();
        private MouseState previousMouseState;
        private MouseState currentMouseState;

        public InputManager() {
            inputToKeyboardMap.Add(GameInput.Up, Keys.Up);
            inputToKeyboardMap.Add(GameInput.Down, Keys.Down);
            inputToKeyboardMap.Add(GameInput.Left, Keys.Left);
            inputToKeyboardMap.Add(GameInput.Right, Keys.Right);
        }

        public double getValue(GameInput input) {
            return 0;
        }

        private bool isPress(GameInput input, State state) {
            KeyboardState keyboardState = currentKeyboardState;
            MouseState mouseState = currentMouseState;
            if (state == State.Previous) {
                keyboardState = previousKeyboardState;
                mouseState = previousMouseState;
            }

            // map to keyboard
            Keys key;
            bool isKeyMapped = inputToKeyboardMap.TryGetValue(input, out key);
            if (isKeyMapped)
                return keyboardState.IsKeyDown(key);

            // map to mouse
            string buttonName;
            ButtonState buttonState = ButtonState.Released;
            isKeyMapped = inputToMouseButtonMap.TryGetValue(input, out buttonName);
            if (isKeyMapped) {
                if (buttonName == "left") buttonState = mouseState.LeftButton;
                else if (buttonName == "right") buttonState = mouseState.RightButton;
                else if (buttonName == "middle") buttonState = mouseState.MiddleButton;
                else if (buttonName == "x1") buttonState = mouseState.XButton1;
                else if (buttonName == "x2") buttonState = mouseState.XButton2;

                return buttonState == ButtonState.Pressed;
            }

            // map to gamepad

            // no mapping found
            return false;
        }

        public bool isPressing(GameInput input) {
            return isPress(input, State.Current);
        }

        public bool isTriggered(GameInput input) {
            return isPress(input, State.Current) && !isPress(input, State.Previous);
        }

        public bool isReleased(GameInput input) {
            return !isPress(input, State.Current) && isPress(input, State.Previous);
        }

        public void Update() {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }
    }
}
