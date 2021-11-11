using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VismaKart.Content;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.Scenes.SceneInfrastructure;
using VismaKart.Utils;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using ZXing;

namespace VismaKart.Scenes
{
    public class JavazoneRegistration : BaseScene
    {
        private Texture2D _grayTexture;
        private SpriteFont _gdprFont;
        private Texture2D _checkmarkTexture;
        private Texture2D _whiteCircleTexture;
        private Texture2D _buttonATexture;
        private Texture2D _buttonBTexture;
        private Texture2D _buttonCTexture;

        private static readonly HttpClient Client = new HttpClient();

        private Texture2D Gdpr1Texture
        {
            get
            {
                if (CurrentPlayer == Player.Player1)
                {
                    return _playerOneGdpr1 ? _checkmarkTexture : _whiteCircleTexture;
                }
                else
                {
                    return _playerTwoGdpr1 ? _checkmarkTexture : _whiteCircleTexture;
                }

            }
        }
        private Texture2D Gdpr2Texture
        {
            get
            {
                if (CurrentPlayer == Player.Player1)
                {
                    if (_playerOneGdpr2)
                    {
                        return _checkmarkTexture;
                    }
                    else
                    {
                        return _whiteCircleTexture;
                    }
                }
                else
                {
                    if (_playerTwoGdpr2)
                    {
                        return _checkmarkTexture;
                    }
                    else
                    {
                        return _whiteCircleTexture;
                    }
                }

            }
        }

        private double _countdownSeconds;
        private string PlayerText
        {
            get
            {
                var player = CurrentPlayer == Player.Player1 ? "1" : "2";
                return $"Player {player}";
            }
        }

        private const string AgreeText = "I want to participate in the competition";
        private const string DisagreeText = "I just want to play";

        private const string GdprText1 = "Yes, I agree to the processing of my personal data \n\r to identify my interest.";
        private const string GdprText2 = "Yes, I want Visma to email me relevant information \n\r based on my topics of interests.";
        private const string GdprCanselText = "I do not accept.";
        private const string PleaseScanCodeText = "Please scan your QR code";
        private const string PlayerOneReadyText = "Player one is ready!";
        private const string PlayerTwoReadyText = "Player two is ready!";
        private bool _playerOneReady;
        private bool _playerTwoReady;
        private bool _playerOneGdpr1;
        private bool _playerOneGdpr2;
        private bool _playerTwoGdpr1;
        private bool _playerTwoGdpr2;
        private MediaCapture _captureMgr;
        private bool _isCameraFound;
        private BarcodeReader _reader;
        private string _player1QrCode;
        private string _player2QrCode;
        private Player CurrentPlayer => _playerOneReady ? Player.Player2 : Player.Player1;
        private RegistrationState _currentRegistrationState = RegistrationState.ContestAgreement;
        private const double QrCodeIntervalTime = 1500;
        private double _millisecondsTillNextQrCodeScan = QrCodeIntervalTime;
        private enum RegistrationState
        {
            ContestAgreement,
            GdprAgreement,
            QrCodeScan
        }

        private enum Player
        {
            Player1,
            Player2
        }

        public JavazoneRegistration(Game game, IPlayerController playerController, ICarsController carsController, IGoalController goalController) : base(game, playerController, carsController, goalController)
        {
            InitializeMediaCapture();
        }

        public override void LoadContent()
        {
            _countdownSeconds = 4;
            _grayTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _grayTexture.SetData(new[] { VismaKartColors.GrayBackground });
            _gdprFont = Game.Content.Load<SpriteFont>("Font/GDPRFont");

            _checkmarkTexture = Game.Content.Load<Texture2D>("Textures/Checkbox");
            _whiteCircleTexture = Game.Content.Load<Texture2D>("Textures/Circle");
            _buttonATexture = Game.Content.Load<Texture2D>("Textures/Button_A");
            _buttonBTexture = Game.Content.Load<Texture2D>("Textures/Button_B");
            _buttonCTexture = Game.Content.Load<Texture2D>("Textures/Button_C");
        }

        public override void Update(GameTime gameTime)
        {
            if (_playerOneReady && _playerTwoReady)
            {
                CountDownState(gameTime);
            }
            else if (_currentRegistrationState == RegistrationState.ContestAgreement)
            {
                ContestAgreementState(gameTime);
            }
            else if (_currentRegistrationState == RegistrationState.GdprAgreement)
            {
                GdprAgreementState(gameTime);
            }
            else if (_currentRegistrationState == RegistrationState.QrCodeScan)
            {
                QrCodeRegistrationState(gameTime);
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

        private void ContestAgreementState(GameTime gameTime)
        {
            if (CurrentPlayer == Player.Player1)
            {
                var player1Input = _playerController.GetPlayer1Input();
                if (player1Input == Button.Red)
                {
                    _currentRegistrationState = RegistrationState.GdprAgreement;
                }
                else if (player1Input == Button.Yellow)
                {
                    _playerOneReady = true;
                }
            }
            else if (CurrentPlayer == Player.Player2)
            {
                var player2Input = _playerController.GetPlayer2Input();
                if (player2Input == Button.Red)
                {
                    _currentRegistrationState = RegistrationState.GdprAgreement;
                }
                else if (player2Input == Button.Yellow)
                {
                    _playerTwoReady = true;
                }
            }
        }

        private void GdprAgreementState(GameTime gameTime)
        {
            var player1Input = _playerController.GetPlayer1Input();
            var player2Input = _playerController.GetPlayer2Input();
            if (CurrentPlayer == Player.Player1)
            {
                if (player1Input == Button.Green)
                {
                    _playerOneGdpr1 = false;
                    _playerOneGdpr2 = false;
                    _currentRegistrationState = RegistrationState.ContestAgreement;
                }
                if (_playerOneGdpr1 && _playerOneGdpr2)
                {
                    _currentRegistrationState = RegistrationState.QrCodeScan;
                }
                else if (player1Input == Button.Red)
                {
                    _playerOneGdpr1 = !_playerOneGdpr1;
                }
                else if (player1Input == Button.Yellow)
                {
                    _playerOneGdpr2 = !_playerOneGdpr2;
                }
            }
            else if (CurrentPlayer == Player.Player2)
            {
                if (player2Input == Button.Green)
                {
                    _playerTwoGdpr1 = false;
                    _playerTwoGdpr2 = false;
                    _currentRegistrationState = RegistrationState.ContestAgreement;
                }
                if (_playerTwoGdpr1 && _playerTwoGdpr2)
                {
                    _currentRegistrationState = RegistrationState.QrCodeScan;
                }
                else if (player2Input == Button.Red)
                {
                    _playerTwoGdpr1 = !_playerTwoGdpr1;
                }
                else if (player2Input == Button.Yellow)
                {
                    _playerTwoGdpr2 = !_playerTwoGdpr2;
                }
            }
        }

        private void QrCodeRegistrationState(GameTime gameTime)
        {
            if (CurrentPlayer == Player.Player1)
            {
                var player1Input = _playerController.GetPlayer1Input();
                if (player1Input == Button.Green)
                {
                    _playerOneGdpr1 = false;
                    _playerOneGdpr2 = false;
                    _currentRegistrationState = RegistrationState.ContestAgreement;
                }
                if (string.IsNullOrEmpty(_player1QrCode))
                {
                    QrCodeLoop(gameTime);
                }
                else
                {
                    GlobalGameState.PlayerOne.Id = _player1QrCode;
                    try
                    {
                        //GlobalGameState.PlayerOne.PostQrCodeTask = Client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/{_player1QrCode}/true", new StringContent(""));

                        //_client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/{player1QrCode}/true/enqueue", new StringContent("")).GetAwaiter().GetResult();

                        var data = JavaZoneQrParser.ParseQrCode(_player1QrCode);

                        GlobalGameState.PlayerOne.Name = data.Name;
                            
                        _playerOneReady = true;
                        _currentRegistrationState = RegistrationState.ContestAgreement;
                    }
                    catch { }
                }
            }
            else if (CurrentPlayer == Player.Player2)
            {
                var player2Input = _playerController.GetPlayer2Input();
                if (player2Input == Button.Green)
                {
                    _playerTwoGdpr1 = false;
                    _playerTwoGdpr2 = false;
                    _currentRegistrationState = RegistrationState.ContestAgreement;
                }
                if (string.IsNullOrEmpty(_player2QrCode))
                {
                    QrCodeLoop(gameTime);
                }
                else
                {
                    GlobalGameState.PlayerTwo.Id = _player2QrCode;
                    try
                    {
                        //GlobalGameState.PlayerTwo.PostQrCodeTask = Client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/{_player2QrCode}/true", new StringContent(""));

                        //_client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/{player2QrCode}/true/enqueue", new StringContent("")).GetAwaiter().GetResult();


                        var data = JavaZoneQrParser.ParseQrCode(_player2QrCode);

                        GlobalGameState.PlayerTwo.Name = data.Name;

                        _playerTwoReady = true;
                        _currentRegistrationState = RegistrationState.ContestAgreement;
                    }
                    catch { }
                }
            }
        }

        private void QrCodeLoop(GameTime gameTime)
        {
            if (_millisecondsTillNextQrCodeScan <= 0)
            {
                ReadQrData();
                _millisecondsTillNextQrCodeScan = QrCodeIntervalTime;
            }
            else
            {
                _millisecondsTillNextQrCodeScan -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public override void DrawSprites(GameTime gameTime)
        {
            var firstTextPositionY = -250;
            var buttonHeight = 100;
            var circleHeight = 80;
            var allPlayersAreReady = _playerOneReady && _playerTwoReady;
            SpriteBatch.Draw(_grayTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), VismaKartColors.GrayBackground);

            if (!allPlayersAreReady && _currentRegistrationState == RegistrationState.ContestAgreement)
            {
                var agreeTextFontSize = _gdprFont.MeasureString(AgreeText);
                SpriteBatch.Draw(_buttonATexture, Center.GetRectanglePositionFromCenter(Game, -((agreeTextFontSize.X / 2) + 325 + 20), firstTextPositionY - (buttonHeight / 2) + (agreeTextFontSize.Y / 2), 325, buttonHeight), Color.White);
                SpriteBatch.Draw(_buttonBTexture, Center.GetRectanglePositionFromCenter(Game, -((agreeTextFontSize.X / 2) + 325 + 20), firstTextPositionY - (buttonHeight / 2) + (agreeTextFontSize.Y / 2) + 325 / 2, 325, buttonHeight), Color.White);
                SpriteBatch.DrawString(_gdprFont, AgreeText, Center.GetVectorPositionFromCenter(Game, -(agreeTextFontSize.X / 2), firstTextPositionY), Color.Black);
                SpriteBatch.DrawString(_gdprFont, DisagreeText, Center.GetVectorPositionFromCenter(Game, -(agreeTextFontSize.X / 2), firstTextPositionY + 325 / 2), Color.Black);

            }
            else if (_currentRegistrationState == RegistrationState.GdprAgreement || _currentRegistrationState == RegistrationState.QrCodeScan)
            {
                var gdprText1FontSize = _gdprFont.MeasureString(GdprText1);
                var gdprText2FontSize = _gdprFont.MeasureString(GdprText2);
                var gdprCanselTextFontSize = _gdprFont.MeasureString(GdprCanselText);
                var pleaseScanCodeTextFont = _gdprFont.MeasureString(PleaseScanCodeText);
                var gdprText1PositionX = -(gdprText1FontSize.X / 2);
                var buttonPositionX = gdprText1PositionX - 325 - 20;
                var circlePositionX = buttonPositionX - buttonHeight;
                var secondTextPositionY = firstTextPositionY + (int)(buttonHeight * 1.5);
                var thirdTextPositionY = secondTextPositionY + (int)(buttonHeight * 1.5);
                var fourthTextPositionY = thirdTextPositionY + 150;

                if (_currentRegistrationState != RegistrationState.QrCodeScan)
                {
                    SpriteBatch.Draw(_buttonATexture, Center.GetRectanglePositionFromCenter(Game, buttonPositionX, firstTextPositionY + (gdprText1FontSize.Y / 2) - (buttonHeight / 2), 325, buttonHeight), Color.White);
                    SpriteBatch.Draw(_buttonBTexture, Center.GetRectanglePositionFromCenter(Game, buttonPositionX, secondTextPositionY + (gdprText2FontSize.Y / 2) - (buttonHeight / 2), 325, buttonHeight), Color.White);
                }

                SpriteBatch.Draw(Gdpr1Texture, Center.GetRectanglePositionFromCenter(Game, circlePositionX, firstTextPositionY + (gdprText1FontSize.Y / 2) - (circleHeight / 2), 80, circleHeight), Color.White);
                SpriteBatch.DrawString(_gdprFont, GdprText1, Center.GetVectorPositionFromCenter(Game, gdprText1PositionX, firstTextPositionY), Color.Black);


                SpriteBatch.Draw(Gdpr2Texture, Center.GetRectanglePositionFromCenter(Game, circlePositionX, secondTextPositionY + (gdprText2FontSize.Y / 2) - (circleHeight / 2), 80, circleHeight), Color.White);
                SpriteBatch.DrawString(_gdprFont, GdprText2, Center.GetVectorPositionFromCenter(Game, gdprText1PositionX, secondTextPositionY), Color.Black);

                SpriteBatch.Draw(_buttonCTexture, Center.GetRectanglePositionFromCenter(Game, buttonPositionX, thirdTextPositionY + (gdprCanselTextFontSize.Y / 2) - (buttonHeight / 2), 325, buttonHeight), Color.White);
                SpriteBatch.DrawString(_gdprFont, GdprCanselText, Center.GetVectorPositionFromCenter(Game, gdprText1PositionX, thirdTextPositionY), Color.Black);

                if (_currentRegistrationState == RegistrationState.QrCodeScan)
                {
                    SpriteBatch.DrawString(_gdprFont, PleaseScanCodeText, Center.GetVectorPositionFromCenter(Game, -(pleaseScanCodeTextFont.X / 2), fourthTextPositionY), Color.Red);
                }
            }
            if (allPlayersAreReady)
            {
                var countDownText = "Starting in " + ((int)_countdownSeconds);
                var countDownTextFont = _gdprFont.MeasureString(countDownText);
                SpriteBatch.DrawString(_gdprFont, countDownText, Center.GetVectorPositionFromCenter(Game, -(countDownTextFont.X / 2), 0), Color.Black);
            }
            else
            {
                var playerTextFontSize = _gdprFont.MeasureString(PlayerText);
                SpriteBatch.DrawString(_gdprFont, PlayerText, Center.GetVectorPositionFromCenter(Game, -(playerTextFontSize.X / 2), firstTextPositionY - 80), GetPlayerColor(CurrentPlayer));
            }

            if (_playerOneReady)
            {
                SpriteBatch.DrawString(_gdprFont, PlayerOneReadyText, new Vector2(100, 100), GetPlayerColor(Player.Player1));
            }
            if (_playerTwoReady)
            {
                var playerTwoReadyTextFont = _gdprFont.MeasureString(PlayerTwoReadyText);
                SpriteBatch.DrawString(_gdprFont, PlayerTwoReadyText, new Vector2(Game.GraphicsDevice.Viewport.Width - playerTwoReadyTextFont.X - 100, 100), GetPlayerColor(Player.Player2));
            }
        }

        private Color GetPlayerColor(Player player)
        {
            return player == Player.Player1 ? Color.DarkBlue : Color.DarkRed;
        }

        #region QRCodeStuff
        private async void InitializeMediaCapture()
        {
            try
            {
                _captureMgr = new MediaCapture();
                var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


                // Use the front camera if found one 
                if (devices == null || devices.Count == 0)
                {
                    _isCameraFound = false;
                    return;
                }

                var info = devices[0];

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = info.Id };


                await _captureMgr.InitializeAsync(settings);


                var resolutions = _captureMgr.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).ToList();
                await _captureMgr.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, resolutions[1]);
                _isCameraFound = true;

                _reader = new BarcodeReader
                {
                    Options =
                    {
                        TryHarder = false
                    }
                };
            }
            catch (Exception ex)
            {
                GC.Collect();
            }
        }

        private async void ReadQrData()
        {
            if (!_isCameraFound) return;
            try
            {
                var currentPlayer = CurrentPlayer;
                var imgFormat = ImageEncodingProperties.CreateJpeg();
                using (var ras = new InMemoryRandomAccessStream())
                {
                    await _captureMgr.CapturePhotoToStreamAsync(imgFormat, ras);
                    var decoder = await BitmapDecoder.CreateAsync(ras);
                    using (var bmp = await decoder.GetSoftwareBitmapAsync())
                    {
                        Result result = await Task.Run(() =>
                        {
                            var source = new SoftwareBitmapLuminanceSource(bmp);
                            return _reader.Decode(source);
                        });
                        if (!string.IsNullOrEmpty(result?.Text))
                        {
                            if (currentPlayer == Player.Player1)
                            {
                                _player1QrCode = result.Text;

                            }
                            else if (currentPlayer == Player.Player2)
                            {
                                _player2QrCode = result.Text;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        #endregion
    }
}
