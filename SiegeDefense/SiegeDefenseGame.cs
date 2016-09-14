using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SiegeDefense {
    public class SiegeDefenseGame : Game {
        GraphicsDeviceManager graphicDeviceManager;
        GameManager gameManager;
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

            gameManager.LoadLevel("level1");

            base.Initialize();
        }

        protected override void LoadContent() {
            base.LoadContent();
        }

        private void RegisterServices() {
            // 2D - SpriteBatch
            Services.AddService(new SpriteBatch(GraphicsDevice));

            // 3D - Effect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.EnableDefaultLighting();
            Services.AddService(basicEffect);

            advancedEffect = Content.Load<Effect>("customShader");
            Services.AddService(advancedEffect);

            // Sound
            SoundBankManager soundManager = new SoundBankManager();
            Services.AddService(soundManager);

            // input
            Services.AddService(typeof(IInputManager), new InputManager());

            // game manager
            gameManager = new GameManager();
            Services.AddService(gameManager);
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
