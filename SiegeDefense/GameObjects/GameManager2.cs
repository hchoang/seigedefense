using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public partial class GameManager {
        public HUD playerStartPointHUD { get; set; }

        public void LoadMapEditorMode(Texture2D mapData) {
            ChangeGame(GameState.MAP_EDITOR_MODE);

            sky = new Skybox();
            map = new HeightMap(mapData);
            Game.Components.Add(sky);
            Game.Components.Add(map);

            mainCamera = new RTSCamera(new Vector3(0, 0, 0), 100, -60);
            Game.Components.Add(mainCamera);

            _2DRenderer playerStartPointRenderer = new _2DRenderer();
            _2DRenderer playerFlag = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\PlayerStartPoint"));

        }
    }
}
