using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiegeDefense {
    public enum GameState {
        TITLE_SCREEN,
        MAP_EDITOR_SELECTION,
        MAP_EDITOR_MODE,
        SURVIVAL_GAME,
        DEFENSE_GAME // unimplemented
    }

    public class GameManager : GameObject
    {
        protected GameState gameState;

        // sound & input
        protected SoundEffectInstance bgm;
        protected SoundBankManager soundManager;
        protected InputManager inputManager;

        private void ChangeGame(GameState newState) {
            // Clear all components
            Game.Components.Clear();

            // Add managers
            soundManager = Game.Services.GetService<SoundBankManager>();
            inputManager = (InputManager)Game.Services.GetService<IInputManager>();
            Game.Components.Add(this);
            Game.Components.Add(inputManager);

            gameState = newState;
        }

        public void LoadTitleScreen() {
            ChangeGame(GameState.TITLE_SCREEN);

            Game.Components.Add(new TitleScreen());

            // play BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public void LoadMapEditorSelectionScreen() {
            ChangeGame(GameState.MAP_EDITOR_SELECTION);

            Game.Components.Add(new MapEditorScreen());

            // play BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

        public void LoadMapEditorMode(Texture2D mapData) {
            ChangeGame(GameState.MAP_EDITOR_MODE);

            MapEditorManager mem = new MapEditorManager();
            Game.Components.Add(mem);
            mem.LoadMap(mapData);
        }

        public void LoadLevel(string levelname) {
            ChangeGame(GameState.SURVIVAL_GAME);

            GameLevelManager glm = new GameLevelManager();
            Game.Components.Add(glm);
            glm.LoadLevel(levelname);

            // Add BGM
            bgm = soundManager.FindSound(SoundType.InBattleBGM).CreateInstance();
            bgm.IsLooped = true;
        }

    }
}