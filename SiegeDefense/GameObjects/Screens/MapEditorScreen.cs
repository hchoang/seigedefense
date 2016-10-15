using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SiegeDefense {
    public class MapEditorScreen : GameObject {

        public _2DRenderer mapPreview { get; set; }
        public VerticalList<TerrainDescription> terrainList { get; set; }
        public TerrainDescription selectedTerrain { get; set; }
        public HUD editMapButton { get; set; }
        
        public MapEditorScreen() {
            // background
            _2DRenderer background = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuBackground"));
            AddComponent(background);

            // map preview
            mapPreview = new _2DRenderer();
            mapPreview.position = new Vector2(0.1f, 0.02f);
            mapPreview.size = new Vector2(0.4f, 0.4f);
            AddComponent(mapPreview);

            // terrain list
            _2DRenderer terrainListRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame"));
            terrainListRenderer.position = new Vector2(0.75f, 0.02f);
            terrainListRenderer.size = new Vector2(0.2f, 0.6f);

            Vector2 terrainListPadding = new Vector2(0.1f, 0.1f);
            float nVisibleTerrain = 3.5f;
            terrainList = new VerticalList<TerrainDescription>(terrainListRenderer, terrainListPadding, nVisibleTerrain);
            terrainList.onItemSelected = onTerrainSelected;
            AddComponent(terrainList);

            // fill data to terrain list
            string[] terrainPathList = Directory.GetFiles(Game.Content.RootDirectory + @"\terrain\", "*.bmp", SearchOption.TopDirectoryOnly);
            SpriteFont terrainItemFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");

            foreach (string terrainPath in terrainPathList) {
                ListItem<TerrainDescription> terrainItem = new ListItem<TerrainDescription>();

                // item data
                string terrainName = Path.GetFileNameWithoutExtension(terrainPath);
                Texture2D terrainTexture = Game.Content.Load<Texture2D>(@"Terrain\" + terrainName);
                terrainItem.data = new TerrainDescription();
                terrainItem.data.TerrainName = terrainName;
                terrainItem.data.TerrainTexture = terrainTexture;

                // item display
                terrainItem.renderer = new _2DRenderer();

                _2DRenderer terrainThumbnailRenderer = new SquareSpriteRenderer(terrainTexture, Color.Blue);
                terrainThumbnailRenderer.position = new Vector2(0.05f, 0.2f);
                terrainThumbnailRenderer.size = new Vector2(0, 0.6f);
                terrainItem.renderer.AddChildRenderer(terrainThumbnailRenderer);

                _2DRenderer terrainNameRenderer = new TextRenderer(terrainName);
                terrainNameRenderer.position = new Vector2(0.3f, 0.05f);
                terrainItem.renderer.AddChildRenderer(terrainNameRenderer);

                terrainList.Add(terrainItem);
            }

            // edit map button
            _2DRenderer editMapButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            editMapButtonRenderer.AddChildRenderer(new TextRenderer("Edit"));
            editMapButtonRenderer.position = terrainListRenderer.position + new Vector2(0, terrainListRenderer.size.Y);
            editMapButtonRenderer.size = new Vector2(terrainListRenderer.size.X, terrainListRenderer.size.Y / 8);
            editMapButton = new HUD(editMapButtonRenderer);
            editMapButton.onClick = onEditClicked;
            AddComponent(editMapButton);

            // exit button
            _2DRenderer exitButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            exitButtonRenderer.AddChildRenderer(new TextRenderer("Exit"));
            exitButtonRenderer.position = editMapButtonRenderer.position + new Vector2(0, editMapButtonRenderer.size.Y + 0.2f);
            exitButtonRenderer.size = editMapButtonRenderer.size;

            HUD exitButton = new HUD(exitButtonRenderer);
            exitButton.onClick = onExitClicked;
            AddComponent(exitButton);
        }

        public void onEditClicked(HUD invoker) {
            if (mapPreview.childRenderers.Count == 0) {
                return;
            }

            gameManager.LoadMapEditorMode(selectedTerrain);
        }

        public void onExitClicked(HUD invoker) {
            gameManager.LoadTitleScreen();
        }

        public void onTerrainSelected(HUD invoker, TerrainDescription data) {
            selectedTerrain = data;
            SquareSpriteRenderer ssr = new SquareSpriteRenderer(data.TerrainTexture, Color.Blue);
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
