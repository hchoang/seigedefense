using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SiegeDefense {
    public class MapEditorScreen : GameObject {

        public _2DRenderer mapPreview { get; set; }
        public VerticalList<Texture2D> terrainList { get; set; }
        public MapEditorScreen() {

            mapPreview = new _2DRenderer();
            mapPreview.position = new Vector2(0.1f, 0.02f);
            mapPreview.size = new Vector2(0.4f, 0.4f);
            AddComponent(mapPreview);

            // retrieve all terrains path
            string[] terrainPathList = Directory.GetFiles(Game.Content.RootDirectory + @"\terrain\", "*.bmp", SearchOption.TopDirectoryOnly);

            // create item list
            SpriteFont terrainItemFont = Game.Content.Load<SpriteFont>(@"Fonts\Arial");
            List<ListItem<Texture2D>> terrainItemList = new List<ListItem<Texture2D>>();
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

                terrainItemList.Add(terrainItem);
            }

            // create terrain list
            Vector2 terrainListPosition = new Vector2(0.75f, 0.02f);
            Vector2 terrainListSize = new Vector2(0.2f, 0.6f);
            Vector2 terrainListPadding = new Vector2(0.1f, 0.1f);
            int nVisibleTerrain = 5;
            Texture2D terrainFrame = Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame");

            terrainList = new VerticalList<Texture2D>(terrainItemList, terrainListPosition, terrainListSize, terrainListPadding, nVisibleTerrain, terrainFrame);
            terrainList.onItemSelected = onTerrainSelected;
            AddComponent(terrainList);
        }

        public void onTerrainSelected(HUD invoker, Texture2D data) {
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
