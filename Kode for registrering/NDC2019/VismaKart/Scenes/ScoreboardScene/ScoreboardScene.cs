using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.Scenes.SceneInfrastructure;
using VismaKart.Scenes.ScoreboardScene.Models;
using VismaKart.Utils;

namespace VismaKart.Scenes.ScoreboardScene
{
    public class ScoreboardScene : BaseScene
    {
        private const int MaxSeconds = 10;
        private const int MinSecondsBetweenHttpCalls = 15;

        private static List<Highscore> _highScores = DefaultHighscoreList.Default;
        private static DateTime _timeForLastHighscoreUpdate = DateTime.MinValue;

        private SpriteFont _font;

        private bool _haveStartedUpdate;

        private Texture2D _scoreboardScreen;
        private readonly DateTime _startedSceneTime;
        private bool _firstUpdate;

        public ScoreboardScene(Game game, IPlayerController playerController, ICarsController carsController,
            IGoalController gc)
            : base(game, playerController, carsController, gc)
        {
            _startedSceneTime = DateTime.Now;
            _firstUpdate = true;
        }

        public override void LoadContent()
        {
            _scoreboardScreen = Game.Content.Load<Texture2D>("Bilder/highscore");
            _font = Game.Content.Load<SpriteFont>("Font/HighscorePixels");
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();

            if (_playerController.GetPlayer1Input() != Button.None || _playerController.GetPlayer2Input() != Button.None)
            {
                //GlobalGameState.CurrentState = GameState.PlayerRegistration;
                //GlobalGameState.CurrentState = GameState.BedExPlayerRegistration;
                GlobalGameState.CurrentState = GameState.JavazoneRegistration;
            }

            if ((DateTime.Now - _startedSceneTime).TotalSeconds > MaxSeconds)
            {
                GlobalGameState.CurrentState = GameState.SplashScreen;
                return;
            }

            UpdateHighscoreIfYouFeelLikeItMaybe();
        }

        private void UpdateHighscoreIfYouFeelLikeItMaybe()
        {
            if (!_firstUpdate && _timeForLastHighscoreUpdate.AddSeconds(MinSecondsBetweenHttpCalls) > DateTime.Now) return;
            _firstUpdate = false;

            if (_haveStartedUpdate)
            {
                return;
            }

            _timeForLastHighscoreUpdate = DateTime.Now;
            try
            {
                _haveStartedUpdate = true;

                Task.Run(() =>
                {
                    List<Highscore> realHighscores;
                    try
                    {
                        realHighscores = GetNewHighScores();
                    }
                    catch
                    {
                        realHighscores = GetNewHighScoresFromStorage();
                    }

                    realHighscores.AddRange(DefaultHighscoreList.Default);
                    _highScores = realHighscores.OrderByDescending(x => x.score).ToList();

                    _haveStartedUpdate = false;
                });
            }
            catch (Exception)
            {
                _haveStartedUpdate = false;
            }
        }

        private List<Highscore> GetNewHighScores()
        {
            var client = HttpClientFactory.Create();

            var result = client.GetAsync("https://vismakart.azurewebsites.net/api/participant")
                .GetAwaiter()
                .GetResult();


            var highscores = JsonConvert.DeserializeObject<List<Highscore>>(
                result.Content
                    .ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult());

            return highscores;
        }

        private List<Highscore> GetNewHighScoresFromStorage()
        {
            var highscoreList = new List<Highscore>();
            try
            {
                var storageFolder = ApplicationData.Current.LocalFolder;
                var scorefile = storageFolder
                    .GetFileAsync("score.txt")
                    .GetAwaiter().GetResult();

                var lines = FileIO.ReadLinesAsync(scorefile)
                    .GetAwaiter().GetResult();


                foreach (var line in lines)
                {
                    var nameAndScore = line.Split('\t');
                    var name = nameAndScore[0];
                    var score = nameAndScore[1];

                    highscoreList.Add(new Highscore
                    {
                        FirstName = name,
                        score = int.Parse(score)
                    });
                }
            }
            catch { }

            return highscoreList;
        }

        public override void DrawSprites(GameTime gameTime)
        {
            SpriteBatch.Draw(_scoreboardScreen, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);

            if (_highScores == null) return;

            if (_highScores == null)
            {
                return;
            }

            var counter = 0;
            for (var i = 0; i < Math.Min(10, _highScores.Count); i++)
            {

                var x = (float)0.2 * Game.GraphicsDevice.Viewport.Width;

                var y = (float)(0.05 * counter * Game.GraphicsDevice.Viewport.Height) + 400;

                counter++;

                try
                {
                    SpriteBatch.DrawString(
                        _font,
                        string.Format($"{_highScores[i].DisplayName,-25} {_highScores[i].score,6}"),
                        new Vector2(x, y),
                        Color.White);
                }
                catch (Exception)
                {
                    SpriteBatch.DrawString(
                        _font,
                        string.Format($"{"Mystery name",-25} {_highScores[i].score,6}"),
                        new Vector2(x, y),
                        Color.White);
                }
            }
        }
    }
}
