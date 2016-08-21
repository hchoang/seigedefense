using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SiegeDefense.GameComponents;
using System;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.Input {
    public class InputManager : GameObject, IInputManager {
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private MouseState originalMouseState;

        private Point centerPosition;
        private Point manuallySetPosition;
        private bool resetCursorPosition = false;

        private enum MouseButton {
            LeftButton,
            RightButton,
            MiddleButton,
            XButton1,
            XButton2
        }

        public override void Initialize() {
            centerPosition = new Point(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            Mouse.SetPosition(centerPosition.X, centerPosition.Y);
            originalMouseState = Mouse.GetState();
            
            //Game.IsMouseVisible = true;
        }

        public override void Update(GameTime gameTime) {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (resetCursorPosition) {
                resetCursorPosition = false;
                manuallySetPosition = Point.Zero;
            }

            if (!GraphicsDevice.Viewport.Bounds.Contains(currentMouseState.Position)) {
                Mouse.SetPosition(centerPosition.X, centerPosition.Y);
                manuallySetPosition = currentMouseState.Position - centerPosition;
                resetCursorPosition = true;
            }

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public float GetValue(GameInput input) {
            return GetValue(input, true);
        }

        private float GetValue(GameInput input, bool isCurrent = true) {

            // Logical to physical input mapping
            if (input == GameInput.Up)
                return GetValue(Keys.W, isCurrent);

            if (input == GameInput.Down)
                return GetValue(Keys.S, isCurrent);

            if (input == GameInput.Left)
                return GetValue(Keys.A, isCurrent);

            if (input == GameInput.Right)
                return GetValue(Keys.D, isCurrent);

            if (input == GameInput.Zoom)
                return GetMouseScroll(false) - GetMouseScroll(true);

            if (input == GameInput.Vertical)
                return GetMouseMovement().Y;

            if (input == GameInput.Horizontal)
                return GetMouseMovement().X;

            if (input == GameInput.Jump)
                return GetValue(Keys.Space, isCurrent);

            if (input == GameInput.Fire)
            {
                return GetValue(MouseButton.LeftButton, isCurrent);
            }
            return 0;
        }

        public bool isPressing(GameInput input) {
            return GetValue(input) != 0;
        }

        public bool isTriggered(GameInput input) {
            bool ret = GetValue(input, false) == 0 && GetValue(input) != 0;

            return ret;
        }

        public bool isReleased(GameInput input) {
            return GetValue(input, false) != 0 && GetValue(input) == 0;
        }

        private float GetValue(Keys key, bool isCurrent) {
            KeyboardState kbs = isCurrent ? currentKeyboardState : previousKeyboardState;

            return kbs.IsKeyDown(key) ? 1 : 0;
        }

        private float GetValue(MouseButton button, bool isCurrent) {
            Console.Out.WriteLine("mouse clicked");
            MouseState ms = isCurrent ? currentMouseState : previousMouseState;
            ButtonState state = (ButtonState)typeof(MouseState).GetProperty(button.ToString()).GetValue(ms);
            return state == ButtonState.Pressed ? 1 : 0;
        }

        private float GetMouseScroll(bool isCurrent) {
            MouseState ms = isCurrent ? currentMouseState : previousMouseState;
            return ms.ScrollWheelValue;
        }

        private Point GetMouseMovement() {
            return currentMouseState.Position - previousMouseState.Position + manuallySetPosition;
        }

        private Vector2 GetMousePosition(bool isCurrent) {
            MouseState ms = isCurrent ? currentMouseState : previousMouseState;
            return ms.Position.ToVector2();
        }
    }
}
