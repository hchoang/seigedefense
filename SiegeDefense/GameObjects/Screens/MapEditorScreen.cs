using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SiegeDefense {
    public class MapEditorScreen : GameObject {

        public _2DRenderer mapPreview { get; set; }
        public VerticalList<Texture2D> terrainList { get; set; }
        public HUD editMapButton { get; set; }
        public Texture2D selectedTerrain { get; set; }
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
            terrainList = new VerticalList<Texture2D>(terrainListRenderer, terrainListPadding, nVisibleTerrain);
            terrainList.onItemSelected = onTerrainSelected;
            AddComponent(terrainList);

            // fill data to terrain list
            string[] terrainPathList = Directory.GetFiles(Game.Content.RootDirectory + @"\terrain\", "*.bmp", SearchOption.TopDirectoryOnly);
            SpriteFont terrainItemFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");

            foreach (string terrainPath in terrainPathList) {
                ListItem<Texture2D> terrainItem = new ListItem<Texture2D>();

                // item data
                string terrainName = Path.GetFileNameWithoutExtension(terrainPath);
                Texture2D terrainData = Game.Content.Load<Texture2D>(@"Terrain\" + terrainName);
                terrainItem.data = terrainData;

                // item display
                terrainItem.renderer = new _2DRenderer();

                _2DRenderer terrainThumbnailRenderer = new SquareSpriteRenderer(terrainData, Color.Blue);
                terrainThumbnailRenderer.position = new Vector2(0.05f, 0.2f);
                terrainThumbnailRenderer.size = new Vector2(0, 0.6f);
                terrainItem.renderer.AddChildRenderer(terrainThumbnailRenderer);

                _2DRenderer terrainNameRenderer = new TextRenderer(terrainName);
                terrainNameRenderer.position = new Vector2(0.3f, 0.05f);
                terrainItem.renderer.AddChildRenderer(terrainNameRenderer);

                terrainList.Add(terrainItem);
            }

            // edit map button
            _2DRenderer editMapButtonRenderer = new _2DRenderer();
            editMapButtonRenderer.AddChildRenderer(new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton")));
            editMapButtonRenderer.AddChildRenderer(new TextRenderer("Edit"));
            editMapButtonRenderer.position = terrainListRenderer.position + new Vector2(0, terrainListRenderer.size.Y);
            editMapButtonRenderer.size = new Vector2(terrainListRenderer.size.X, terrainListRenderer.size.Y / 8);
            editMapButton = new HUD(editMapButtonRenderer);
            editMapButton.onClick = onEditClicked;
            AddComponent(editMapButton);
        }

        public void onEditClicked(HUD invoker) {
            if (mapPreview.childRenderers.Count == 0) {
                return;
            }

            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadMapEditorMode(selectedTerrain);
        }

        public void onTerrainSelected(HUD invoker, Texture2D data) {
            selectedTerrain = data;
            SquareSpriteRenderer ssr = new SquareSpriteRenderer(data, Color.Blue);
            ssr.size = new Vector2(1, 0.2f);
            if (mapPreview.childRenderers.Count == 0) {
                mapPreview.AddChildRenderer(ssr);
            } else {
                mapPreview.childRenderers[0] = ssr;
                ssr.parentRenderer = mapPreview;
            }
        }
    }
}
