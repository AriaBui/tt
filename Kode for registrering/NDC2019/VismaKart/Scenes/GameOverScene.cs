using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VismaKart.Content;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.Player;
using VismaKart.Scenes.SceneInfrastructure;

namespace VismaKart.Scenes
{
    public class GameOverScene : BaseScene
    {
        private SpriteFont _gameOverFont;
        private Texture2D _grayTexture;
        private Texture2D _gameOverTexture;

        public GameOverScene(Game game, IPlayerController playerController, ICarsController carsController, IGoalController goalController)
            : base(game, playerController, carsController, goalController)
        {
        }

        public override void LoadContent()
        {
            _gameOverFont = Game.Content.Load<SpriteFont>("Font/GameOverResultFont");
            _gameOverTexture = Game.Content.Load<Texture2D>("Textures/Game-Over");
            _grayTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _grayTexture.SetData(new[] { VismaKartColors.GrayBackground });
        }

        public override void Update(GameTime gameTime)
        {
            if (_playerController.GetPlayer1Input() != Button.None ||
                _playerController.GetPlayer2Input() != Button.None)
            {
                GlobalGameState.PlayerOne = new VismaKartPlayer("Player 1");
                GlobalGameState.PlayerTwo = new VismaKartPlayer("Player 2");
                GlobalGameState.CurrentState = GameState.SplashScreen;
                //GlobalGameState.CurrentState = GameState.QuizGame;
            }

            _carsController.StopCar1();
            _carsController.StopCar2();
        }

        public override void DrawSprites(GameTime gameTime)
        {

            //SpriteBatch.Draw(_grayTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), VismaKartColors.GrayBackground);
            SpriteBatch.Draw(_gameOverTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), VismaKartColors.GrayBackground);

            var winner = GlobalGameState.PlayerOne.Score > GlobalGameState.PlayerTwo.Score
                ? GlobalGameState.PlayerOne
                : GlobalGameState.PlayerTwo;

            var looser = GlobalGameState.PlayerOne.Score < GlobalGameState.PlayerTwo.Score
                ? GlobalGameState.PlayerOne
                : GlobalGameState.PlayerTwo;

            SpriteBatch.DrawString(_gameOverFont,
                $"1   {winner.Name}     {winner.Score} points",
                new Vector2(
                    //Game.GraphicsDevice.Viewport.Width / 2 - 150,
                    200,
                    Game.GraphicsDevice.Viewport.Height / 2 - 175),
                Color.White);

            SpriteBatch.DrawString(_gameOverFont,
                $"2   {looser.Name}     {looser.Score} points",
                new Vector2(
                    //Game.GraphicsDevice.Viewport.Width / 2 - 150,
                    200,
                    Game.GraphicsDevice.Viewport.Height / 2 + 50),
                Color.White);
        }

        private string GetResultText()
        {
            var playerOne = GlobalGameState.PlayerOne;
            var playerTwo = GlobalGameState.PlayerTwo;
            if (playerOne.Score == playerTwo.Score)
            {
                return "It's a draw!";
            }

            return $"The winner is {(playerOne.Score < playerTwo.Score ? playerTwo.Name : playerOne.Name)}!";
        }
    }
}
