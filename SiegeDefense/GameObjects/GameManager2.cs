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
        public HUD enemySpawnPointHUD { get; set; }
        public _3DGameObject playerStartPointMaker { get; set; }
        public List<_3DGameObject> enemySpawnPointMakers { get; set; }
        public void LoadMapEditorMode(Texture2D mapData) {
            ChangeGame(GameState.MAP_EDITOR_MODE);

            sky = new Skybox();
            map = new HeightMap(mapData);
            Game.Components.Add(sky);
            Game.Components.Add(map);

            mainCamera = new RTSCamera(new Vector3(0, 0, 0), 100, -60);
            Game.Components.Add(mainCamera);

            // player start point
            _2DRenderer playerStartPointRenderer = new _2DRenderer();
            playerStartPointRenderer.position = new Vector2(0.05f, 0.9f);
            playerStartPointRenderer.size = new Vector2(0.1f, 0.1f);

            _2DRenderer playerFlag = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\PlayerStartPoint"));
            playerFlag.size = new Vector2(0.7f, 0.7f);
            playerFlag.position = new Vector2(0.15f, 0);
            playerStartPointRenderer.AddChildRenderer(playerFlag);

            _2DRenderer playerStartPointText = new TextRenderer("Player start point");
            playerStartPointText.position = new Vector2(-0.2f, 0.7f);
            playerStartPointRenderer.AddChildRenderer(playerStartPointText);

            playerStartPointHUD = new HUD(playerStartPointRenderer);
            playerStartPointHUD.onClick = SelectPlayerStartPoint;
            Game.Components.Add(playerStartPointHUD);

            // enemy spawn point
            _2DRenderer enemySpawnPointRenderer = new _2DRenderer();
            enemySpawnPointRenderer.position = new Vector2(0.25f, 0.9f);
            enemySpawnPointRenderer.size = new Vector2(0.1f, 0.1f);

            _2DRenderer enemyFlag = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\EnemySpawnPoint"));
            enemyFlag.size = new Vector2(0.7f, 0.7f);
            enemyFlag.position = new Vector2(0.15f, 0);
            enemySpawnPointRenderer.AddChildRenderer(enemyFlag);

            _2DRenderer enemySpawnPointText = new TextRenderer("Enemy spawn point");
            enemySpawnPointText.position = new Vector2(-0.2f, 0.7f);
            enemySpawnPointRenderer.AddChildRenderer(enemySpawnPointText);

            enemySpawnPointHUD = new HUD(enemySpawnPointRenderer);
            enemySpawnPointHUD.onClick = SelectedEnemySpawnPoint;
            Game.Components.Add(enemySpawnPointHUD);
        }

        public void SelectPlayerStartPoint(HUD invoker) {
            if (playerStartPointMaker != null) {
                Game.Components.Remove(playerStartPointMaker);
            }
            
            playerStartPointMaker = new _3DGameObject();
            playerStartPointMaker.Tag = "PlayerStartPoint";
            playerStartPointMaker.renderer = new BillboardRenderer(Game.Content.Load<Texture2D>(@"Sprites\PlayerStartPoint"));
            playerStartPointMaker.AddComponent(playerStartPointMaker.renderer);
            playerStartPointMaker.collider = new Collider(new BoundingBox(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 1, 0.5f)));
            playerStartPointMaker.AddComponent(new PlayerStartPointController());
            playerStartPointMaker.transformation.ScaleMatrix = Matrix.CreateScale(5);
            Game.Components.Add(playerStartPointMaker);
        }

        public void SelectedEnemySpawnPoint(HUD invoker) {
            _3DGameObject enemyMaker = new _3DGameObject();
            playerStartPointMaker.Tag = "EnemySpawnPoint";
            enemyMaker.renderer = new BillboardRenderer(Game.Content.Load<Texture2D>(@"Sprites\EnemySpawnPoint"));
            enemyMaker.AddComponent(enemyMaker.renderer);
            enemyMaker.collider = new Collider(new BoundingBox(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 1, 0.5f)));
            enemyMaker.AddComponent(new EnemySpawPointController());
            enemyMaker.transformation.ScaleMatrix = Matrix.CreateScale(5);
            Game.Components.Add(enemyMaker);
        }
    }
}
