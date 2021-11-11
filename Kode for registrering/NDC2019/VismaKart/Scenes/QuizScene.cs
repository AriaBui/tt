using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VismaKart.Content;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.QnA;
using VismaKart.Scenes.QuizSceneState;
using VismaKart.Scenes.SceneInfrastructure;
using VismaKart.Utils;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.Storage;
using Newtonsoft.Json;
using VismaKart.Player;

namespace VismaKart.Scenes
{
    public class QuizScene : BaseScene
    {
        public const int SecondsPerQuestions = 8;
        public const int SecondsThatGiveMaxScore = 2;

        public const int MaxPossibleScore = 1000;
        public const int MinScoreWithCorrectAnswer = 100;

        private Speedometer _speedometer;

        private const int SecondsBeforeNewQuestion = 4;

        private const int TopTime = (SecondsPerQuestions - SecondsThatGiveMaxScore);
        private const int ScorePerSecond = (MaxPossibleScore - MinScoreWithCorrectAnswer) / TopTime;

        private double _currentPossibleScore;

        private readonly Color _signBackground = new Color(173, 93, 50);

        private readonly TimeSpan _runCarsFor = TimeSpan.FromMilliseconds(500);

        private readonly QuizSceneState.QuizSceneState _state;
        private readonly QuestionProvider _questionProvider;

        private SpriteFont _questionFont;
        private SpriteFont _answerFont;
        private SpriteFont _playerScoreFont;
        private SpriteFont _boldFont;

        private Texture2D _blankTexture;
        private Texture2D _whiteTexture;
        private Texture2D _grayTexture;
        private Texture2D _checkmarkTexture;
        private Texture2D _wrongTexture;
        private Texture2D _whiteCircleTexture;
        private Texture2D _questionBoxTexture;

        private Texture2D _buttonATexture;
        private Texture2D _buttonBTexture;
        private Texture2D _buttonCTexture;
        private Texture2D _buttonDTexture;

        private Vector2 position;
        private Texture2D needleImage;
        private Texture2D backgroundImage;

        public QuizScene(Game game, IPlayerController playerController, QuestionProvider questionProvider, ICarsController carsController, IGoalController goalController)
            : base(game, playerController, carsController, goalController)
        {
            _state = new QuizSceneState.QuizSceneState
            {
                QuizState = QuizState.StartNewGame,
                CurrentQuestionNumber = 0
            };

            _questionProvider = questionProvider;
        }

        public override void LoadContent()
        {
            // Fonts
            _questionFont = Game.Content.Load<SpriteFont>("Font/QuestionFont");
            _answerFont = Game.Content.Load<SpriteFont>("Font/AnswerFont");
            _playerScoreFont = Game.Content.Load<SpriteFont>("Font/PlayerScore");
            _boldFont = Game.Content.Load<SpriteFont>("Font/BoldFont");



            // Textures
            _checkmarkTexture = Game.Content.Load<Texture2D>("Textures/Checkbox");
            _wrongTexture = Game.Content.Load<Texture2D>("Textures/Wrong");
            _whiteCircleTexture = Game.Content.Load<Texture2D>("Textures/Circle");
            _questionBoxTexture = Game.Content.Load<Texture2D>("Textures/QuestionBoxRound");

            _buttonATexture = Game.Content.Load<Texture2D>("Textures/Button_A_empty");
            _buttonBTexture = Game.Content.Load<Texture2D>("Textures/Button_B_empty");
            _buttonCTexture = Game.Content.Load<Texture2D>("Textures/Button_C_empty");
            _buttonDTexture = Game.Content.Load<Texture2D>("Textures/Button_D_empty");

            _blankTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _whiteTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _grayTexture = new Texture2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);

            _blankTexture.SetData(new[] { _signBackground });
            _whiteTexture.SetData(new[] { Color.White });
            _grayTexture.SetData(new[] { VismaKartColors.GrayBackground });

            //GameComponents
            backgroundImage = Game.Content.Load<Texture2D>("Textures/spm");
            position = new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - (backgroundImage.Width / 4), 100);
            needleImage = Game.Content.Load<Texture2D>("Textures/needle3");
            _speedometer = new Speedometer(position, backgroundImage, needleImage, SpriteBatch, 0.5f);
        }

        public override void Update(GameTime gameTime)
        {
            //run cars
            _carsController.UpdateCars(gameTime);

            //check if a car have reach the finish line
            if (_gc is NumberOfQuestionsGoalController debugGC)
            {
                debugGC.updateGoalController(_state.CurrentQuestionNumber, GlobalGameState.PlayerOne.Score, GlobalGameState.PlayerTwo.Score);
            }


            var player1Goal = _gc.Player1IsInGoal();
            var player2Goal = _gc.Player2IsInGoal();
            if (player1Goal || player2Goal)
            {
                //SendScore();
                StoreScoreLocally();
                GlobalGameState.CurrentState = GameState.GameOver;
            }


            if (_state.QuizState == QuizState.StartNewGame)
            {
                _state.QuizState = QuizState.NewQuestion;
            }

            if (_state.QuizState == QuizState.NewQuestion)
            {
                _state.CurrentQuestion = _questionProvider.GetNextQuestion();
                _state.CurrentAnswers = AnswerPicker.GetFourAnswersWithAtLeastOneCorrect(_state.CurrentQuestion);
                _state.QuizState = QuizState.GameRunning;
                _state.CurrentQuestionNumber++;
                _state.TimeToEnd = DateTime.Now.Add(TimeSpan.FromSeconds(SecondsPerQuestions));
                GlobalGameState.PlayerOne.CorrectAnswer = false;
                GlobalGameState.PlayerOne.HasAnswered = false;
                GlobalGameState.PlayerTwo.CorrectAnswer = false;
                GlobalGameState.PlayerTwo.HasAnswered = false;
            }

            if (_state.QuizState == QuizState.GameRunning)
            {
                var secondsLeft = (_state.TimeToEnd - DateTime.Now).TotalMilliseconds / 1000;
                _state.SecondsLeft = $"{secondsLeft:0}";
                _speedometer.Update((float)secondsLeft, SecondsPerQuestions);

                if (secondsLeft >= TopTime)
                {
                    _currentPossibleScore = MaxPossibleScore;
                }
                else
                {
                    _currentPossibleScore = Math.Max(100, (int)(secondsLeft * ScorePerSecond) + 100);
                }

                // Check if players have answered correctly
                var player1Input = _playerController.GetPlayer1Input();
                var player2Input = _playerController.GetPlayer2Input();


                if (!GlobalGameState.PlayerOne.HasAnswered)
                {
                    if (player1Input != Button.None)
                    {
                        GlobalGameState.PlayerOne.HasAnswered = true;

                        GlobalGameState.PlayerOne.CorrectAnswer = CheckIfCorrectAnswer(player1Input);

                        GlobalGameState.PlayerOne.UpdateScoreThisRound();
                    }
                }

                if (!GlobalGameState.PlayerTwo.HasAnswered)
                {
                    if (player2Input != Button.None)
                    {
                        GlobalGameState.PlayerTwo.HasAnswered = true;

                        GlobalGameState.PlayerTwo.CorrectAnswer = CheckIfCorrectAnswer(player2Input);

                        GlobalGameState.PlayerTwo.UpdateScoreThisRound();
                    }
                }

                if (GlobalGameState.PlayerOne.HasAnswered)
                {
                    if (!GlobalGameState.PlayerOne.CorrectAnswer)
                    {
                        GlobalGameState.PlayerOne.MaxScorePossibleThisRound = 0;
                    }
                }
                else
                {
                    GlobalGameState.PlayerOne.MaxScorePossibleThisRound = _currentPossibleScore;
                }

                if (GlobalGameState.PlayerTwo.HasAnswered)
                {
                    if (!GlobalGameState.PlayerTwo.CorrectAnswer)
                    {
                        GlobalGameState.PlayerTwo.MaxScorePossibleThisRound = 0;
                    }
                }
                else
                {
                    GlobalGameState.PlayerTwo.MaxScorePossibleThisRound = _currentPossibleScore;
                }


                if (DateTime.Now >= _state.TimeToEnd || (GlobalGameState.PlayerOne.HasAnswered && GlobalGameState.PlayerTwo.HasAnswered))
                {
                    _state.SecondsLeft = "0";
                    _state.QuizState = QuizState.TimerExpired;
                    _state.TimeToEnd = DateTime.Now.Add(TimeSpan.FromSeconds(SecondsBeforeNewQuestion));

                    GlobalGameState.PlayerOne.UpdateScore();
                    GlobalGameState.PlayerTwo.UpdateScore();

                    if (GlobalGameState.PlayerOne.CorrectAnswer)
                    {
                        _carsController.ToggleRunCar1(gameTime, _runCarsFor);
                    }

                    if (GlobalGameState.PlayerTwo.CorrectAnswer)
                    {
                        _carsController.ToggleRunCar2(gameTime, _runCarsFor);
                    }
                }
            }

            if (_state.QuizState == QuizState.TimerExpired)
            {

                if (DateTime.Now >= _state.TimeToEnd)
                {
                    _state.QuizState = QuizState.NewQuestion;

                }
            }
        }

        public override void DrawSprites(GameTime gameTime)
        {
            const int questionBoxHeight = 400;
            const int questionBoxWidth = 1250;

            const int answerBoxWidth = (questionBoxWidth - 125) / 2;
            const int answerBoxHeight = questionBoxHeight / 2;


            const int playerScoreBoxSize = 120;

            const int questionBoxPositionX = -questionBoxWidth / 2;
            var questionBoxPositionY = -questionBoxHeight / 2 - Game.GraphicsDevice.Viewport.Height / 4 + 20;

            var timerPosY = questionBoxPositionY - 60;

            const int answerBoxColumn1X = questionBoxPositionX;
            const int answerBoxColumn2X = answerBoxColumn1X + answerBoxWidth + 125;

            var answerBoxRow1Y = questionBoxHeight + questionBoxPositionY + 50;
            var answerBoxRow2Y = answerBoxRow1Y + questionBoxHeight / 2 + 50;

            const int player1ScoreX = questionBoxPositionX - 200;
            const int player2ScoreX = questionBoxWidth / 2 + 200 - playerScoreBoxSize;
            const int playerScoresY = -200;

            // Background
            SpriteBatch.Draw(_grayTexture, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);


            // TODO: Create a Taunting phrase factory that can generate taunts if the players get it wrong, or right
            if (_state.QuizState == QuizState.TimerExpired)
            {
                string taunt;
                if (GlobalGameState.PlayerOne.CorrectAnswer && GlobalGameState.PlayerTwo.CorrectAnswer)
                {
                    taunt = "Both are correct!";
                }
                else if (GlobalGameState.PlayerOne.CorrectAnswer)
                {
                    taunt = $"{GlobalGameState.PlayerOne.Name} pulls ahead!";
                }
                else if (GlobalGameState.PlayerTwo.CorrectAnswer)
                {
                    taunt = $"{GlobalGameState.PlayerTwo.Name} gets it right!";
                }
                else
                {
                    taunt = "You both need to try harder";
                }

                SpriteBatch.DrawString(_boldFont, taunt,
                    Center.GetHorizontallyCenteredVectorWithText(Game, questionBoxPositionX, timerPosY, questionBoxWidth, _boldFont, taunt),
                    Color.Black);
            }
            else
            {
                // Timer
                //SpriteBatch.DrawString(_boldFont, _state.SecondsLeft + " s",
                //    Center.GetHorizontallyCenteredVectorWithText(Game, questionBoxPositionX, timerPosY, questionBoxWidth, _boldFont, _state.SecondsLeft + " s"),
                //    Color.Black);
            }

            //Player score
            DrawPlayerScore(player1ScoreX, playerScoresY, GlobalGameState.PlayerOne, playerScoreBoxSize, Color.DarkBlue);
            DrawPlayerScore(player2ScoreX + 10, playerScoresY, GlobalGameState.PlayerTwo, playerScoreBoxSize, Color.DarkRed);

            //Draw checkmark if a player have answered
            if (GlobalGameState.PlayerOne.HasAnswered)
            {
                if (GlobalGameState.PlayerOne.CorrectAnswer)
                {
                    SpriteBatch.Draw(_checkmarkTexture,
                        Center.GetRectanglePositionFromCenter(Game, player1ScoreX + 10, 0, playerScoreBoxSize - 20,
                            playerScoreBoxSize - 20), Color.White);
                }
                else
                {
                    SpriteBatch.Draw(_wrongTexture,
                        Center.GetRectanglePositionFromCenter(Game, player1ScoreX + 10, 0, playerScoreBoxSize - 20,
                            playerScoreBoxSize - 20), Color.White);
                }
            }

            //Draw checkmark if a player have answered
            if (GlobalGameState.PlayerTwo.HasAnswered)
            {
                if (GlobalGameState.PlayerTwo.CorrectAnswer)
                {
                    SpriteBatch.Draw(_checkmarkTexture,
                        Center.GetRectanglePositionFromCenter(Game, player2ScoreX + 10, 0, playerScoreBoxSize - 20,
                            playerScoreBoxSize - 20), Color.White);
                }
                else
                {
                    SpriteBatch.Draw(_wrongTexture,
                        Center.GetRectanglePositionFromCenter(Game, player2ScoreX + 10, 0, playerScoreBoxSize - 20,
                            playerScoreBoxSize - 20), Color.White);
                }
            }

            // QuestionBox
            //SpriteBatch.Draw(_questionBoxTexture, Center.GetRectanglePositionFromCenter(Game, questionBoxPositionX, questionBoxPositionY, questionBoxWidth, questionBoxHeight), Color.White);


            // Question
            SpriteBatch.DrawString(_questionFont, WordWrap.WrapText(
                _questionFont,
                _state.CurrentQuestion.Text,
                questionBoxWidth - 50),
                Center.GetHorizontallyAndVerticallyCenteredVectorWithTextWrap
                    (Game, questionBoxPositionX + 20, questionBoxPositionY + 20, questionBoxWidth, questionBoxHeight, _questionFont, _state.CurrentQuestion.Text),
                Color.Black);

            // Answer boxes
            DrawAnswerBox(answerBoxColumn1X, answerBoxRow1Y, answerBoxWidth, answerBoxHeight, _state.CurrentQuestion.Answers[0], AnswerLetter.A, Color.Red);
            DrawAnswerBox(answerBoxColumn2X, answerBoxRow1Y, answerBoxWidth, answerBoxHeight, _state.CurrentQuestion.Answers[1], AnswerLetter.B, Color.Yellow);

            DrawAnswerBox(answerBoxColumn1X, answerBoxRow2Y, answerBoxWidth, answerBoxHeight, _state.CurrentQuestion.Answers[2], AnswerLetter.C, Color.Green);
            DrawAnswerBox(answerBoxColumn2X, answerBoxRow2Y, answerBoxWidth, answerBoxHeight, _state.CurrentQuestion.Answers[3], AnswerLetter.D, Color.Blue);
            _speedometer.Draw();
        }

        private void DrawPlayerScore(int x, int y, VismaKartPlayer player, int playerScoreBoxSize, Color playerColor)
        {
            // Hvit boks
            SpriteBatch.Draw(_whiteCircleTexture, Center.GetRectanglePositionFromCenter(Game, x, y, playerScoreBoxSize, playerScoreBoxSize), Color.White);

            // Current possible score
            SpriteBatch.DrawString(_playerScoreFont, ((int)player.MaxScorePossibleThisRound).ToString(),
                Center.GetHorizontallyCenteredVectorWithText(Game, x + 10, y - 80, playerScoreBoxSize, _answerFont, ((int)player.MaxScorePossibleThisRound).ToString()),
                Color.Black);

            // Modifier
            SpriteBatch.DrawString(_playerScoreFont, "x " + player.CurrentScoreBoost,
                Center.GetHorizontallyCenteredVectorWithText(Game, x, y - 40, playerScoreBoxSize, _answerFont, "x " + player.CurrentScoreBoost),
                Color.Black);

            // Current score
            SpriteBatch.DrawString(_playerScoreFont, player.Score.ToString(),
                Center.GetVectorPositionFromCenter(Game, x + playerScoreBoxSize / 2 - _answerFont.MeasureString(player.Score.ToString()).X / 2, y + playerScoreBoxSize / 2 - _answerFont.MeasureString(player.ScoreThisRound.ToString()).Y / 2 - 2),
                Color.Black);

            try
            {
                SpriteBatch.DrawString(_boldFont, Regex.Replace(player.Name, @"[^a-zA-Z ]+", string.Empty),
                    Center.GetHorizontallyCenteredVectorWithText(Game, x - 20, y + playerScoreBoxSize,
                        playerScoreBoxSize, _answerFont, player.Name),
                    playerColor);
            }
            catch
            {
                player.Name = "Anonymous";
                SpriteBatch.DrawString(_boldFont, player.Name,
                    Center.GetHorizontallyCenteredVectorWithText(Game, x - 20, y + playerScoreBoxSize,
                        playerScoreBoxSize, _answerFont, player.Name),
                    playerColor);
            }
        }

        private enum AnswerLetter
        {
            A, B, C, D
        }

        private void DrawAnswerBox(int x, int y, int width, int height, Answer answer, AnswerLetter answerLetter, Color answerColor)
        {

            //Draw marker for right answer
            if (_state.QuizState == QuizState.TimerExpired && !answer.Correct)
            {
                //SpriteBatch.Draw(_checkmarkTexture, Center.GetRectanglePositionFromCenter(x + 9 , y + 31, 35, 35), Color.White);
                return;
            }

            Texture2D texture;
            switch (answerLetter)
            {
                case AnswerLetter.A:
                    texture = _buttonATexture;
                    break;
                case AnswerLetter.B:
                    texture = _buttonBTexture;
                    break;
                case AnswerLetter.C:
                    texture = _buttonCTexture;
                    break;
                case AnswerLetter.D:
                    texture = _buttonDTexture;
                    break;
                default:
                    texture = _whiteTexture;
                    break;
            }

            //SpriteBatch.Draw(_whiteTexture, Center.GetRectanglePositionFromCenter(x, y, width, height), Color.White);
            SpriteBatch.Draw(texture, Center.GetRectanglePositionFromCenter(Game, x, y, width, height), Color.White);

            SpriteBatch.DrawString(_answerFont, WordWrap.WrapText(_answerFont, answer.Text, width - 100),
                Center.GetHorizontallyAndVerticallyCenteredVectorWithTextWrap(Game, x + 100, y, width - 100, height, _answerFont, answer.Text),
                Color.Black);
        }

        /// <summary>
        /// Assumes that Red button = answer 0, yellow answer 1, green answer 2 and 3 answer blue
        /// </summary>
        /// <param name="pressedButton"></param>
        /// <returns></returns>
        private bool CheckIfCorrectAnswer(Button pressedButton)
        {
            switch (pressedButton)
            {
                case Button.Red:
                    return _state.CurrentAnswers[0].Correct;
                case Button.Yellow:
                    return _state.CurrentAnswers[1].Correct;
                case Button.Green:
                    return _state.CurrentAnswers[2].Correct;
                case Button.Blue:
                    return _state.CurrentAnswers[3].Correct;
                default:
                    return false;
            }
        }

        private async void SendScore()
        {
            try
            {
                var playerOne = GlobalGameState.PlayerOne;
                var playerTwo = GlobalGameState.PlayerTwo;

                var client = HttpClientFactory.Create();
                if (!string.IsNullOrEmpty(playerOne.Id))
                {
                    await client.PutAsync($"https://vismakart.azurewebsites.net/api/participant/{playerOne.Id}/{playerOne.Score}", new StringContent(""));
                }
                if (!string.IsNullOrEmpty(playerTwo.Id))
                {
                    await client.PutAsync($"https://vismakart.azurewebsites.net/api/participant/{playerTwo.Id}/{playerTwo.Score}", new StringContent(""));
                }
            }
            catch { }
        }

        private void StoreScoreLocally()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var scorefile = storageFolder
                    .CreateFileAsync("score.txt", CreationCollisionOption.OpenIfExists)
                    .GetAwaiter().GetResult();

                var playerOne = GlobalGameState.PlayerOne;
                var playerTwo = GlobalGameState.PlayerTwo;

                //client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/bedex",
                //        new StringContent($"{{\"Name\": \"{playerOne.Name}\", \"Score\": {playerOne.Score}}}", Encoding.UTF8, "application/json"), tokenSrc.Token)
                //    .GetAwaiter().GetResult();

                //client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/bedex",
                //        new StringContent($"{{\"Name\": \"{playerTwo.Name}\", \"Score\": {playerTwo.Score}}}", Encoding.UTF8, "application/json"), tokenSrc.Token)
                //    .GetAwaiter().GetResult();

                if (playerOne.Id != null)
                {
                    StorePlayerScore(scorefile, playerOne);
                }

                if (playerTwo.Id != null)
                {
                    StorePlayerScore(scorefile, playerTwo);
                }
            }
            catch { }
        }

        private static void StorePlayerScore(IStorageFile scorefile, VismaKartPlayer player)
        {

            try
            {
                FileIO.AppendTextAsync(scorefile, $"{player.Name}\t{player.Score}\t{player.Id}\r\n")
                    .GetAwaiter().GetResult();
            }
            catch { }
            try
            {
                var p = JavaZoneQrParser.ParseQrCode(player.Id);
                p.Score = player.Score;

                var client = HttpClientFactory.Create();

                var tokenSrc = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                var result = client.PostAsync($"https://vismakart.azurewebsites.net/api/participant/javazone",
                        new StringContent(
                            JsonConvert.SerializeObject(p),
                            Encoding.UTF8,
                            "application/json"),
                        tokenSrc.Token)
                    .GetAwaiter().GetResult();
            } catch { }
        }
    }
}
