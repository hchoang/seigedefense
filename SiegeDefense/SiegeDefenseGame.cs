using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SiegeDefense.GameObjects;
using SiegeDefense.GameObjects.Map;
using SiegeDefense.GameObjects.TitleScreen;
using SiegeDefense.Input;
using System.Collections.Generic;

namespace SiegeDefense {
    public class SiegeDefenseGame : Game {
        GraphicsDeviceManager graphicDeviceManager;
        Stack<GameObject> rootObjectStack = new Stack<GameObject>();
        GameObject inputManager = new InputManager();
        Camera mainCamera;
        BasicEffect effect;

        public SiegeDefenseGame() {
            graphicDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            base.Initialize();

            graphicDeviceManager.PreferredBackBufferWidth = 1024;
            graphicDeviceManager.PreferredBackBufferHeight = 768;
            graphicDeviceManager.ApplyChanges();
            IsMouseVisible = true;

            GameObject.Initialize(this);

            RegisterServices();

            //rootObjectStack.Push(new TitleScreen());
            rootObjectStack.Push(HeightMap.createFromTexture(Content.Load<Texture2D>(@"Sprites\terrain"), 1));
        }

        private void RegisterServices() {

            // Graphics device
            Services.AddService(graphicDeviceManager);

            // Input
            Services.AddService(typeof(IInputManager), inputManager);

            // 2D - SpriteBatch
            Services.AddService(new SpriteBatch(GraphicsDevice));

            // 3D
            // Camera
            mainCamera = new Camera(new Vector3(20, 200, 0), new Vector3(100, 0, 100), Vector3.Up);
            Services.AddService(typeof(Camera), mainCamera);

            // Effect
            effect = new BasicEffect(GraphicsDevice);
            effect.View = mainCamera.ViewMatrix;
            effect.Projection = mainCamera.ProjectionMatrix;
            effect.EnableDefaultLighting();
            Services.AddService(effect);

            Services.AddService(rootObjectStack);
        }
        
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mainCamera.Update(gameTime);
            effect.Projection = mainCamera.ProjectionMatrix;
            inputManager.Update(gameTime);
            rootObjectStack.Peek().Update(gameTime);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            inputManager.Draw(gameTime);
            rootObjectStack.Peek().Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
