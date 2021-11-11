using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VismaKart.Content;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.Scenes.SceneInfrastructure;
using VismaKart.Utils;

namespace VismaKart.Scenes
{
    public class BedExPlayerRegistration : BaseScene
    {
        private Texture2D _grayTexture;
        private SpriteFont _gdprFont;
        private SpriteFont _boldFont;

        private double _countdownSeconds;
        private string PlayerText
        {
            get
            {
                var player = CurrentPlayer == Player.Player1 ? "1" : "2";
                return $"Player {player}";
            }
        }

        private bool _playerOneReady = false;
        private bool _playerTwoReady = false;
        private int groupNumber = 0;
        private const int maxGroupNumber = 20;
        private Player CurrentPlayer => _playerOneReady ? Player.Player2 : Player.Player1;

        private enum Player
        {
            Player1,
            Player2
        }

        public BedExPlayerRegistration(Game game, IPlayerController playerController, ICarsController carsController, IGoalController goalController) : base(game, playerController, carsController, goalController)
        {
        }

        public override void LoadContent()
        {
            _countdownSeconds = 4;
            _grayTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _grayTexture.SetData(new[] { VismaKartColors.GrayBackground });
            _gdprFont = Game.Content.Load<SpriteFont>("Font/GDPRFont");
            _boldFont = Game.Content.Load<SpriteFont>("Font/BoldFont");
        }

        private bool _isKeyBDown;
        private bool _isKeyEnterDown;
        public override void Update(GameTime gameTime)
        {
            if (_playerOneReady && _playerTwoReady)
            {
                CountDownState(gameTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T) && !_isKeyBDown)
            {
                _isKeyBDown = true;
                if (maxGroupNumber == groupNumber)
                    groupNumber = 0;
                groupNumber++;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.T))
            {
                _isKeyBDown = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_isKeyEnterDown)
            {
                _isKeyEnterDown = true;
                if (!_playerOneReady)
                {
                    GlobalGameState.PlayerOne.Name = $"Group {groupNumber}";
                    _playerOneReady = true;
                }
                else if (_playerOneReady)
                {
                    GlobalGameState.PlayerTwo.Name = $"Group {groupNumber}";
                    _playerTwoReady = true;
                }
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                _isKeyEnterDown = false;
            }
        }

        private void CountDownState(GameTime gameTime)
        {
            if (_countdownSeconds <= 0)
            {
                GlobalGameState.CurrentState = GameState.QuizGame;
            }
            else
            {
                _countdownSeconds -= gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void DrawSprites(GameTime gameTime)
        {
            var countDownText = "Starting in " + ((int)_countdownSeconds);
            var countDownTextFont = _gdprFont.MeasureString(countDownText);
            var countdownPositionX = -(countDownTextFont.X / 2);
            SpriteBatch.Draw(_grayTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), VismaKartColors.GrayBackground);
            if(_playerOneReady)
                SpriteBatch.DrawString(_gdprFont, GlobalGameState.PlayerOne.Name, Center.GetVectorPositionFromCenter(Game, -150 + countdownPositionX/2, -100), Color.Black);
            if(_playerTwoReady)
                SpriteBatch.DrawString(_gdprFont, GlobalGameState.PlayerTwo.Name, Center.GetVectorPositionFromCenter(Game, 150 + countdownPositionX/2, -100), Color.Black);
            if (_playerOneReady && _playerTwoReady)
            {
                SpriteBatch.DrawString(_gdprFont, countDownText, Center.GetVectorPositionFromCenter(Game, countdownPositionX, 0), Color.Black);
            }
            else
            {
                SpriteBatch.DrawString(_gdprFont, groupNumber.ToString(), Center.GetVectorPositionFromCenter(Game, 0, 0), Color.Black);
            }
        }
    }
}
