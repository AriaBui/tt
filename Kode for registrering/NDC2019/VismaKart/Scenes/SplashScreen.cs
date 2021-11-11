using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.Scenes.SceneInfrastructure;

namespace VismaKart.Scenes
{
    public class SplashScreen : BaseScene
    {
        private const int MaxSeconds = 10;
        private Texture2D _splashScreen;
        private DateTime _startedSceneTime;

        public SplashScreen(Game game, IPlayerController playerController, ICarsController carsController,
            IGoalController goalController)
            : base(game, playerController, carsController, goalController)
        {
            _startedSceneTime = DateTime.Now;
        }

        public override void LoadContent()
        {
            _splashScreen = Game.Content.Load<Texture2D>("Textures/start-screen");
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();

            if (_playerController.GetPlayer1Input() != Button.None || _playerController.GetPlayer2Input() != Button.None)
            {
                //GlobalGameState.CurrentState = GameState.BedExPlayerRegistration;
                GlobalGameState.CurrentState = GameState.JavazoneRegistration;
            }

            if ((DateTime.Now - _startedSceneTime).TotalSeconds > MaxSeconds)
            {
                GlobalGameState.CurrentState = GameState.Scoreboard;
            }
        }

        public override void DrawSprites(GameTime gameTime)
        {
            SpriteBatch.Draw(_splashScreen, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            // _spriteBatch.DrawString(_vismaFont, "Press Space to start", new Vector2((float)0.2 * Game.GraphicsDevice.Viewport.Width, (float)0.8 * Game.GraphicsDevice.Viewport.Height), Color.White);
        }
    }
}
