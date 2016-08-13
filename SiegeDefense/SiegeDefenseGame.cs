using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SiegeDefense.GameComponents;
using SiegeDefense.GameComponents.Input;
using System.Collections.Generic;
using SiegeDefense.GameComponents.Cameras;
using SiegeDefense.GameScreens;

namespace SiegeDefense {
    public class SiegeDefenseGame : Game {
        GraphicsDeviceManager graphicDeviceManager;
        GameObject inputManager;
        BasicEffect basicEffect;
        Effect advancedEffect;

        public SiegeDefenseGame() {
            graphicDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            graphicDeviceManager.PreferredBackBufferWidth = 1024;
            graphicDeviceManager.PreferredBackBufferHeight = 768;
            graphicDeviceManager.ApplyChanges();
            //IsMouseVisible = true;

            GameObject.Initialize(this);
            RegisterServices();

            Components.Add(inputManager);
            Components.Add(new MainGameScreen());

            base.Initialize();
        }

        protected override void LoadContent() {
            base.LoadContent();
        }

        private void RegisterServices() {
            // Input
            inputManager = new InputManager();
            Services.AddService(typeof(IInputManager), inputManager);

            // 2D - SpriteBatch
            Services.AddService(new SpriteBatch(GraphicsDevice));

            // 3D - Effect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.EnableDefaultLighting();
            Services.AddService(basicEffect);

            advancedEffect = Content.Load<Effect>("customShader");
            Services.AddService(advancedEffect);

            //mainCamera = new FPSCamera(new Vector3(500, 100, 500), new Vector3(100, 20, 100), Vector3.Up);
            //Services.AddService(mainCamera);
        }
        
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            base.Draw(gameTime);
        }
    }
}
