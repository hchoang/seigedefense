using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class MapEditorManager : GameObject {
        public HUD playerStartPointHUD { get; set; }
        public HUD enemySpawnPointHUD { get; set; }
        public _3DGameObject playerStartPointMaker { get; set; }
        public bool isModalDisplaying { get; set; } = false;
        public TextBox namingTextbox { get; set; }
        public string terrainName { get; set; }

        public void LoadMap(TerrainDescription terrainData) {
            this.terrainName = terrainData.TerrainName;
            Skybox sky = new Skybox();
            Map map = new HeightMap(terrainData.TerrainTexture);
            Game.Components.Add(sky);
            Game.Components.Add(map);

            Camera mainCamera = new RTSCamera(new Vector3(0, 0, 0), 100, -60);
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

            // save button
            _2DRenderer saveButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            saveButtonRenderer.size = new Vector2(0.1f, 0.1f);
            saveButtonRenderer.position = new Vector2(0.7f, 0.9f);
            saveButtonRenderer.AddChildRenderer(new TextRenderer("Save"));

            HUD saveButton = new HUD(saveButtonRenderer);
            saveButton.onClick = SaveButtonClick;
            Game.Components.Add(saveButton);

            // exit button
            _2DRenderer exitButtonRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
            exitButtonRenderer.size = new Vector2(0.1f, 0.1f);
            exitButtonRenderer.position = new Vector2(0.8f, 0.9f);
            exitButtonRenderer.AddChildRenderer(new TextRenderer("Exit"));

            HUD exitButton = new HUD(exitButtonRenderer);
            exitButton.onClick = ExitButtonClick;
            Game.Components.Add(exitButton);
        }

        public void SelectPlayerStartPoint(HUD invoker) {
            if (isModalDisplaying) {
                return;
            }

            if (playerStartPointMaker != null && Game.Components.Contains(playerStartPointMaker)) {
                Game.Components.Remove(playerStartPointMaker);
            }

            playerStartPointMaker = new _3DGameObject();
            playerStartPointMaker.Tag = "PlayerStartPoint";
            playerStartPointMaker.renderer = new BillboardRenderer(Game.Content.Load<Texture2D>(@"Sprites\PlayerStartPoint"));
            playerStartPointMaker.AddComponent(playerStartPointMaker.renderer);
            playerStartPointMaker.collider = new Collider(new BoundingBox(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 1, 0.5f)));
            playerStartPointMaker.AddComponent(new SelectPointController());
            playerStartPointMaker.transformation.ScaleMatrix = Matrix.CreateScale(5);
            Game.Components.Add(playerStartPointMaker);
        }

        public void SelectedEnemySpawnPoint(HUD invoker) {
            if (isModalDisplaying) {
                return;
            }

            _3DGameObject enemyMaker = new _3DGameObject();
            enemyMaker.Tag = "EnemySpawnPoint";
            enemyMaker.renderer = new BillboardRenderer(Game.Content.Load<Texture2D>(@"Sprites\EnemySpawnPoint"));
            enemyMaker.AddComponent(enemyMaker.renderer);
            enemyMaker.collider = new Collider(new BoundingBox(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 1, 0.5f)));
            enemyMaker.AddComponent(new SelectPointController());
            enemyMaker.transformation.ScaleMatrix = Matrix.CreateScale(5);
            Game.Components.Add(enemyMaker);
        }

        public void ExitButtonClick(HUD invoker) {
            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadTitleScreen();
        }

        public void SaveButtonClick(HUD invoker) {
            if (isModalDisplaying) {
                return;
            }

            string warningMessage = null;

            List<GameObject> enemySpawnPoint = FindObjectsByTag("EnemySpawnPoint");
            if (enemySpawnPoint.Count == 0) {
                warningMessage = "Please have at least\none enemy spawn point";
            }

            List<GameObject> playerStartPoint = FindObjectsByTag("PlayerStartPoint");
            if (playerStartPoint.Count == 0) {
                warningMessage = "Please select player start point";
            } else {
                SelectPointController spc = playerStartPoint[0].GetComponent<SelectPointController>()[0];
                if (spc.state == "Selecting") {
                    return;
                }
            }

            _2DRenderer modalBackgroundRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\WhiteBar"));
            modalBackgroundRenderer.color = Color.Black * 0.5f;
            HUD modalBackground = new HUD(modalBackgroundRenderer);
            modalBackground.Tag = "Modal";
            Game.Components.Add(modalBackground);
            isModalDisplaying = true;

            if (warningMessage != null) {
                // display warning message
                _2DRenderer warningBackgroundRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame"));
                TextRenderer warningTextRenderer = new TextRenderer(warningMessage);

                Rectangle backgroundDrawArea = warningBackgroundRenderer.GetDrawArea();
                Vector2 warningTextSize = warningTextRenderer.font.MeasureString(warningMessage);
                Vector2 newBackgroundSize = warningTextSize / backgroundDrawArea.Size.ToVector2() + new Vector2(0.05f, 0.05f);
                Vector2 newBackgroundPosition = (Vector2.One - newBackgroundSize) / 2; // screen center

                warningBackgroundRenderer.position = newBackgroundPosition;
                warningBackgroundRenderer.size = newBackgroundSize;
                warningBackgroundRenderer.AddChildRenderer(warningTextRenderer);

                HUD warningDialog = new HUD(warningBackgroundRenderer);
                warningDialog.Tag = "Modal";
                Game.Components.Add(warningDialog);

                // display ok button
                _2DRenderer okButtonFrame = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
                okButtonFrame.AddChildRenderer(new TextRenderer("OK"));
                okButtonFrame.size = new Vector2(0.05f, 0.05f);
                okButtonFrame.position = new Vector2(0.475f, 0.525f);

                HUD okButton = new HUD(okButtonFrame);
                okButton.Tag = "Modal";
                okButton.onClick = OkButtonClick;
                Game.Components.Add(okButton);

            } else {
                // ask player to name the map
                SpriteRenderer namingFrameRenderer = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuFrame"));
                TextRenderer namingTextRenderer = new TextRenderer("Please enter a name for your map");

                Rectangle namingFrameDrawArea = namingFrameRenderer.GetDrawArea();
                Vector2 namingTextSize = namingTextRenderer.font.MeasureString(namingTextRenderer.text);
                Vector2 newNamingFrameSize = namingTextSize / namingFrameDrawArea.Size.ToVector2() + new Vector2(0.05f, 0.05f);
                Vector2 newNamingFramePosition = (Vector2.One - newNamingFrameSize) / 2;

                namingFrameRenderer.position = newNamingFramePosition;
                namingFrameRenderer.size = newNamingFrameSize;
                namingFrameRenderer.AddChildRenderer(namingTextRenderer);

                HUD namingFrame = new HUD(namingFrameRenderer);
                namingFrame.Tag = "Modal";
                Game.Components.Add(namingFrame);

                // display textbox
                namingTextbox = new TextBox(Color.Green * 0.5f, Color.White, new Vector2(0.3f, 0.55f), new Vector2(0.4f, 0.1f));
                namingTextbox.Tag = "Modal";
                Game.Components.Add(namingTextbox);

                // ok & cancel button
                _2DRenderer okButtonFrame = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
                okButtonFrame.AddChildRenderer(new TextRenderer("OK"));
                okButtonFrame.size = new Vector2(0.08f, 0.08f);
                okButtonFrame.position = new Vector2(0.35f, 0.7f);

                HUD okButton = new HUD(okButtonFrame);
                okButton.Tag = "Modal";
                okButton.onClick = SaveMapButtonClick;
                Game.Components.Add(okButton);

                _2DRenderer cancelButtonFrame = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuButton"));
                cancelButtonFrame.AddChildRenderer(new TextRenderer("Cancel"));
                cancelButtonFrame.size = new Vector2(0.08f, 0.08f);
                cancelButtonFrame.position = new Vector2(0.55f, 0.7f);

                HUD cancelButton = new HUD(cancelButtonFrame);
                cancelButton.Tag = "Modal";
                cancelButton.onClick = CancelButtonClick;
                Game.Components.Add(cancelButton);
            }
        }

        public void SaveMapButtonClick(HUD invoker) {
            if (namingTextbox.textRenderer.text == "") {
                return;
            }
            LevelDescription ld = new LevelDescription();
            ld.PlayerStartPoint = playerStartPointMaker.transformation.Position;
            List<GameObject> enemySpawnPoints = FindObjectsByTag("EnemySpawnPoint");
            foreach (GameObject enemySpawnPoint in enemySpawnPoints) {
                ld.SpawnPoints.Add(enemySpawnPoint.transformation.Position);
            }
            ld.MapCellSize = 10;
            ld.MapDeltaHeight = 200;
            ld.TerrainName = terrainName;
            ld.SaveToXML(Game.Content.RootDirectory + @"\Level\" + namingTextbox.textRenderer.text + ".xml");

            GameManager gameManager = Game.Services.GetService<GameManager>();
            gameManager.LoadTitleScreen();
        }

        public void CancelButtonClick(HUD invoker) {
            List<GameObject> modalComponents = FindObjectsByTag("Modal");
            foreach (GameObject modalComponent in modalComponents) {
                Game.Components.Remove(modalComponent);
            }

            isModalDisplaying = false;
        }

        public void OkButtonClick(HUD invoker) {
            List<GameObject> modalComponents = FindObjectsByTag("Modal");
            foreach (GameObject modalComponent in modalComponents) {
                Game.Components.Remove(modalComponent);
            }

            isModalDisplaying = false;
        }
    }
}
