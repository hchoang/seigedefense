
using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Models;
using System;

namespace SiegeDefense.GameComponents.Input {
    public class TankController : GameObject {
        private IInputManager inputManager;
        private float moveSpeed = 50.0f;
        private float rotateSpeed = 2.0f;

        private Tank _controlledTank;
        private Tank controlledTank {
            get {
                if (_controlledTank == null) {
                    _controlledTank = FindComponent<Tank>();
                }
                return _controlledTank;
            }
        }
        private Map _map;
        private Map map {
            get {
                if (_map == null) {
                    _map = FindObjects<Map>()[0];
                }
                return _map;
            }
        }

        public TankController() {
            inputManager = Game.Services.GetService<IInputManager>();
        }

        public override void Update(GameTime gameTime) {
            Vector3 moveDirection = Vector3.Zero;
            int rotationDirection = 1;
            if (inputManager.GetValue(GameInput.Up) != 0)
                moveDirection -= controlledTank.Forward;
            else if (inputManager.GetValue(GameInput.Down) != 0) {
                moveDirection += controlledTank.Forward;
                rotationDirection = -1;
            }

            if (moveDirection != Vector3.Zero)
                moveDirection = Vector3.Normalize(moveDirection) * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            controlledTank.Move(moveDirection);

            float rotationAngle = 0;
            if (moveDirection != Vector3.Zero) {
                if (inputManager.GetValue(GameInput.Left) != 0) {
                    rotationAngle += rotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (inputManager.GetValue(GameInput.Right) != 0) {
                    rotationAngle -= rotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if (rotationAngle != 0) {
                rotationAngle *= rotationDirection;
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(controlledTank.Up, rotationAngle);
                controlledTank.RotationMatrix *= rotationMatrix;
            }

        }
    }
}
