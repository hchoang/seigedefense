using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiegeDefense {
    public class GameManager : GameObject
    {
        // sound & input
        protected SoundEffectInstance bgm;
        protected SoundBankManager soundManager;
        protected InputManager inputManager;
        public bool _isPaused = false;
        public bool isPaused {
            get {
                return _isPaused;
            } set {
                if (value == false) {
                    List<GameObject> modalComponents = FindObjectsByTag("Modal");
                    foreach (GameObject modalComponent in modalComponents) {
                        Game.Components.Remove(modalComponent);
                    }
                }
                _isPaused = value;
            }
        }

        private void ChangeState() {
            // Clear all components
            Game.Components.Clear();

            // Add managers
            soundManager = Game.Services.GetService<SoundBankManager>();
            inputManager = (InputManager)Game.Services.GetService<IInputManager>();
            Game.Components.Add(this);
            Game.Components.Add(inputManager);

            // reset cursor
            inputManager.toggleCursor(true);
            // reset pause state
            isPaused = false;

            // reset bgm
            if (bgm != null) {
                bgm.Stop();
            }
        }

        public void LoadTitleScreen() {
            ChangeState();

            Game.Components.Add(new TitleScreen());

            // play BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public void LoadSelectLevelScreen() {
            ChangeState();

            Game.Components.Add(new SelectLevelScreen());

            // play BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public void LoadMapEditorSelectionScreen() {
            ChangeState();

            Game.Components.Add(new MapEditorScreen());

            // play BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public void LoadMapEditorMode(TerrainDescription terrainData) {
            ChangeState();

            MapEditorManager mem = new MapEditorManager();
            Game.Components.Add(mem);
            mem.LoadMap(terrainData);
        }

        public void LoadLevel(LevelDescription levelDescrption) {
            ChangeState();

            inputManager.toggleCursor(false);

            GameLevelManager glm = new GameLevelManager();
            Game.Components.Add(glm);
            glm.LoadLevel(levelDescrption);

            // Add BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public override void Update(GameTime gameTime) {
            bgm.Play();

            if (inputManager.isTriggered(GameInput.ToggleBoundingBox)) {
                GameObject.isBoundingBoxDisplay = !GameObject.isBoundingBoxDisplay;
            }

            if (inputManager.isTriggered(GameInput.ToggleHPBar)) {
                GameObject.isHPBarDisplay = !GameObject.isHPBarDisplay;
            }

            if (inputManager.isTriggered(GameInput.TogglePartitioner)) {
                GameObject.isPartitionerDisplay = !GameObject.isPartitionerDisplay;
            }

            if (inputManager.isTriggered(GameInput.ToggleSteeringForce)) {
                GameObject.isSteeringForceDisplay = !GameObject.isSteeringForceDisplay;
            }

            base.Update(gameTime);
        }

    }
}