
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Models;
using SiegeDefense.GameComponents.SoundBank;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.Input {
    public class TankController : GameObject {
        private IInputManager inputManager;
        private float tankMoveSpeed = 50.0f;
        private float tankRotateSpeed = 2.0f;
        private float turretRotateSpeed = 0.05f;
        private float canonRotateSpeed = 0.05f;

        private SoundBankManager _soundManager;
        private SoundBankManager soundManager {
            get
            {
                if (_soundManager == null)
                {
                    _soundManager = FindObjects<SoundBankManager>()[0];
                }
                
                return _soundManager;
            }
        }

        private ModelManager _modelManager;
        private ModelManager modelManager
        {
            get
            {
                if (_modelManager == null)
                {
                    _modelManager = FindObjects<ModelManager>()[0];
                }
                return _modelManager;
            }
        }

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
            // play sound when click mouse
            if (inputManager.isTriggered(GameInput.Fire))
            {
                SoundEffect se = Game.Content.Load<SoundEffect>(@"Sound/tank-fire");
                //se.Play();
                //SoundEffectInstance sound = soundManager.FindSound("Tank");
                //if (sound.State != SoundState.Playing)
                //{
                //    sound.Play();
                //}
                controlledTank.Fire();
                //List<Tank> allTanks = modelManager.models.Where(x => x is Tank).Cast<Tank>().ToList();
                //foreach (Tank tank in allTanks)
                //    tank.Fire();
            }

            // move & rotate
            Vector3 moveDirection = Vector3.Zero;
            int rotationDirection = 1;
            if (inputManager.GetValue(GameInput.Up) != 0)
                moveDirection += controlledTank.Forward;
            else if (inputManager.GetValue(GameInput.Down) != 0) {
                moveDirection -= controlledTank.Forward;
                rotationDirection = -1;
            }

            if (moveDirection != Vector3.Zero) {
                moveDirection = Vector3.Normalize(moveDirection) * tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                controlledTank.Move(moveDirection);
            }

            float rotationAngle = 0;
           // if (moveDirection != Vector3.Zero) {
                if (inputManager.GetValue(GameInput.Left) != 0) {
                    rotationAngle += tankRotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (inputManager.GetValue(GameInput.Right) != 0) {
                    rotationAngle -= tankRotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
          //  }

            if (rotationAngle != 0) {
                rotationAngle *= rotationDirection;
                controlledTank.RotateTank(rotationAngle);
            }
            // end move & rotate

            //rotate wheels
            float travelDistance = moveDirection.Length();
            
            if (travelDistance > 0)
            {
                controlledTank.RotateWheels(travelDistance * rotationDirection);
            }

            // rotate turret & cannon
            float turretRotationAngle = inputManager.GetValue(GameInput.Horizontal) * (float)gameTime.ElapsedGameTime.TotalSeconds * turretRotateSpeed;
            float canonRotationAngle = inputManager.GetValue(GameInput.Vertical) * (float)gameTime.ElapsedGameTime.TotalSeconds * canonRotateSpeed;

            if (turretRotationAngle != 0) {
                controlledTank.RotateTurret(turretRotationAngle);
            }

            if (canonRotationAngle != 0) {
                controlledTank.RotateCanon(canonRotationAngle);
            }
        }
    }
}
