using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SiegeDefense.GameObjects;
using SiegeDefense.GameObjects.TitleScreen;
using SiegeDefense.Input;
using System.Collections.Generic;

namespace SiegeDefense {
    public class SiegeDefenseGame : Game {
        GraphicsDeviceManager graphicDeviceManager;
        Stack<GameObject> rootObjectStack = new Stack<GameObject>();

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

            rootObjectStack.Push(new TitleScreen());
        }

        private void RegisterServices() {

            // Display
            Services.AddService(new SpriteBatch(GraphicsDevice));
            Services.AddService(graphicDeviceManager);

            // Input
            Services.AddService(new InputManager());

            Services.AddService(rootObjectStack);
        }
        
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            rootObjectStack.Peek().Update(gameTime);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            rootObjectStack.Peek().Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
