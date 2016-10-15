using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class SelectLevelScreen : GameObject {

        public _2DRenderer mapPreview { get; set; }
        public VerticalList<LevelDescription> levelList { get; set; }
        public LevelDescription selectedLevel { get; set; }

        public SelectLevelScreen() {
            // background
            _2DRenderer background = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuBackground"));
            AddComponent(background);

            // map preview
            mapPreview = new _2DRenderer();
            mapPreview.position = new Vector2(0.1f, 0.02f);
            mapPreview.size = new Vector2(0.4f, 0.4f);
            AddComponent(mapPreview);

            // level list
            _2DRenderer levelListRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame"));
            levelListRenderer.position = new Vector2(0.75f, 0.02f);
            levelListRenderer.size = new Vector2(0.2f, 0.6f);

            Vector2 levelListPadding = new Vector2(0.1f, 0.1f);
            float nVisibleLevel = 3.5f;
            levelList = new VerticalList<LevelDescription>(levelListRenderer, levelListPadding, nVisibleLevel);
            levelList.onItemSelected = onLevelSelected;
            AddComponent(levelList);

            // fill data to level list
            string[] levelPathList = Directory.GetFiles(Game.Content.RootDirectory + @"\Level\", "*.xml", SearchOption.TopDirectoryOnly);
            SpriteFont levelItemFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");

            foreach (string levelPath in levelPathList) {
                ListItem<LevelDescription> levelItem = new ListItem<LevelDescription>();

                // level data
                string levelName = Path.GetFileNameWithoutExtension(levelPath);
                LevelDescription levelDescription = LevelDescription.LoadFromXML(levelPath);
                levelItem.data = levelDescription;
                Texture2D levelTerrainTexture = Game.Content.Load<Texture2D>(@"Terrain\" + levelDescription.TerrainName);

                // level display
                levelItem.renderer = new _2DRenderer();

                _2DRenderer terrainThumbnailRenderer = new SquareSpriteRenderer(levelTerrainTexture, Color.Blue);
                terrainThumbnailRenderer.position = new Vector2(0.05f, 0.2f);
                terrainThumbnailRenderer.size = new Vector2(0, 0.6f);
                levelItem.renderer.AddChildRenderer(terrainThumbnailRenderer);

                _2DRenderer levelNameRenderer = new TextRenderer(levelName);
                levelNameRenderer.position = new Vector2(0.3f, 0.05f);
                levelItem.renderer.AddChildRenderer(levelNameRenderer);

                levelList.Add(levelItem);
            }

            // play button
            _2DRenderer playButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            playButtonRenderer.AddChildRenderer(new TextRenderer("Play"));
            playButtonRenderer.position = levelListRenderer.position + new Vector2(0, levelListRenderer.size.Y);
            playButtonRenderer.size = new Vector2(levelListRenderer.size.X, levelListRenderer.size.Y / 8);
            HUD playButton = new HUD(playButtonRenderer);
            playButton.onClick = onPlayClicked;
            AddComponent(playButton);

            // exit button
            _2DRenderer exitButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            exitButtonRenderer.AddChildRenderer(new TextRenderer("Exit"));
            exitButtonRenderer.position = new Vector2(0.8f, 0.8f);
            exitButtonRenderer.size = new Vector2(0.1f, 0.1f);

            HUD exitButton = new HUD(exitButtonRenderer);
            exitButton.onClick = onExitClicked;
            AddComponent(exitButton);
        }

        public void onExitClicked(HUD invoker) {
            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadTitleScreen();
        }

        public void onPlayClicked(HUD invoker) {
            if (mapPreview.childRenderers.Count == 0) {
                return;
            }

            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadLevel(selectedLevel);
        }

        public void onLevelSelected(HUD invokder, LevelDescription data) {
            selectedLevel = data;
            SquareSpriteRenderer ssr = new SquareSpriteRenderer(Game.Content.Load<Texture2D>(@"Terrain\" + data.TerrainName), Color.Blue);
            ssr.size = new Vector2(1, 0);
            if (mapPreview.childRenderers.Count == 0) {
                mapPreview.AddChildRenderer(ssr);
            } else {
                mapPreview.childRenderers[0] = ssr;
                ssr.parentRenderer = mapPreview;
            }
        }
    }
}
