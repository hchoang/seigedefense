﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SiegeDefense.GameComponents.Maps;
using SiegeDefense.GameComponents.Models;
using SiegeDefense.GameComponents.SoundBank;
using System;

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
            if (inputManager.GetValue(GameInput.Fire) != 0)
            {
                Console.Out.WriteLine(1);
                SoundEffectInstance sound = soundManager.FindSound("Tank");
                if (sound.State != SoundState.Playing)
                {
                    sound.Play();
                }
            }

            // move & rotate
            Vector3 moveDirection = Vector3.Zero;
            int rotationDirection = 1;
            if (inputManager.GetValue(GameInput.Up) != 0)
                moveDirection -= controlledTank.Forward;
            else if (inputManager.GetValue(GameInput.Down) != 0) {
                moveDirection += controlledTank.Forward;
                rotationDirection = -1;
            }

            if (moveDirection != Vector3.Zero) {
                moveDirection = Vector3.Normalize(moveDirection) * tankMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                controlledTank.Move(moveDirection);
            }

            float rotationAngle = 0;
            if (moveDirection != Vector3.Zero) {
                if (inputManager.GetValue(GameInput.Left) != 0) {
                    rotationAngle += tankRotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (inputManager.GetValue(GameInput.Right) != 0) {
                    rotationAngle -= tankRotateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if (rotationAngle != 0) {
                rotationAngle *= rotationDirection;
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(controlledTank.Up, rotationAngle);
                controlledTank.RotationMatrix *= rotationMatrix;
            }
            // end move & rotate

            //rotate wheels
            float travelDistance = moveDirection.Length();
            
            if (travelDistance > 0)
            {
                for (int i = 0; i < controlledTank.wheelBones.Length; i++)
                {
                    Matrix wheelMatrix = controlledTank.wheelBones[i].Transform;
                    Vector3 wheelForward = wheelMatrix.Forward;
                    Vector3 wheelUp = wheelMatrix.Up;
                    Vector3 position = wheelMatrix.Translation;
                    
                    wheelMatrix = Matrix.CreateFromAxisAngle(wheelMatrix.Right, MathHelper.PiOver4 * 0.25f);
                    wheelForward = Vector3.Transform(wheelForward, wheelMatrix);
                    wheelUp = Vector3.Transform(wheelUp, wheelMatrix);
                    Matrix newFrontWheelMatrix = Matrix.CreateWorld(position, wheelForward, wheelUp);
                    controlledTank.wheelBones[i].Transform = newFrontWheelMatrix;
                }
                
            }

            // rotate turret & cannon
            float turretRotationAngle = inputManager.GetValue(GameInput.Horizontal) * (float)gameTime.ElapsedGameTime.TotalSeconds * turretRotateSpeed;
            float canonRotationAngle = inputManager.GetValue(GameInput.Vertical) * (float)gameTime.ElapsedGameTime.TotalSeconds * canonRotateSpeed;

            if (turretRotationAngle != 0) {
                Matrix turretMatrix = controlledTank.turretBone.Transform;
                Vector3 turretForward = turretMatrix.Forward;
                Vector3 turretPosition = turretMatrix.Translation;
                Vector3 turretUp = turretMatrix.Up;

                Matrix turretRotateMatrix = Matrix.CreateFromAxisAngle(turretMatrix.Up, -turretRotationAngle);
                turretForward = Vector3.Transform(turretForward, turretRotateMatrix);
                turretUp = Vector3.Transform(turretUp, turretRotateMatrix);

                Matrix newTurretMatrix = Matrix.CreateWorld(turretPosition, turretForward, turretUp);
                if (Math.Abs(newTurretMatrix.Rotation.Y) < 0.2f) {
                    controlledTank.turretBone.Transform = newTurretMatrix;
                }
            }

            if (canonRotationAngle != 0) {
                Matrix canonMatrix = controlledTank.canonBone.Transform;
                Vector3 canonForward = canonMatrix.Forward;
                Vector3 canonPosition = canonMatrix.Translation;
                Vector3 canonUp = canonMatrix.Up;

                Matrix canonRotateMatrix = Matrix.CreateFromAxisAngle(canonMatrix.Left, -canonRotationAngle);
                canonForward = Vector3.Transform(canonForward, canonRotateMatrix);
                canonForward.Normalize();
                if (-0.5f < canonForward.Y && canonForward.Y < 0.2f) {
                    canonUp = Vector3.Transform(canonUp, canonRotateMatrix);
                    controlledTank.canonBone.Transform = Matrix.CreateWorld(canonPosition, canonForward, canonUp);
                }
                
            }
        }
    }
}
