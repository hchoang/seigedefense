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

        private void ChangeState() {
            // Clear all components
            Game.Components.Clear();

            // Add managers
            soundManager = Game.Services.GetService<SoundBankManager>();
            inputManager = (InputManager)Game.Services.GetService<IInputManager>();
            Game.Components.Add(this);
            Game.Components.Add(inputManager);
        }

        public void LoadTitleScreen() {
            ChangeState();

            Game.Components.Add(new TitleScreen());

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

        public void LoadMapEditorMode(Texture2D mapData) {
            ChangeState();

            MapEditorManager mem = new MapEditorManager();
            Game.Components.Add(mem);
            mem.LoadMap(mapData);
        }

        public void LoadLevel(string levelname) {
            ChangeState();

            GameLevelManager glm = new GameLevelManager();
            Game.Components.Add(glm);
            glm.LoadLevel(levelname);

            // Add BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

    }
}