using System;
using Windows.System.Profile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;
using VismaKart.QnA;
using VismaKart.Scenes;
using VismaKart.Scenes.SceneInfrastructure;
using VismaKart.Scenes.ScoreboardScene;

namespace VismaKart
{
    public class VismaKart : Game
    {
        // Public static, yow! Fordi det funker. Den trengs egentlig overalt.
        public static GraphicsDeviceManager GraphicsDeviceManager;

        private GameState _lastKnownState = GameState.NotStarted;

        private IScene _scene;

        private IPlayerController _playerController;

        private QuestionProvider _questionProvider;
       
        private ICarsController _carController;
        private IGoalController _gc;

        private bool _isInitalized;
        private int numberOfQuestionsPerRound = 10;
        public VismaKart()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override async void Initialize()
        {
            // TODO: Add your initialization logic here6

            base.Initialize();

            // Vet ikke om noen av disse fungerer. Bare tester!
            GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
            GraphicsDeviceManager.PreferredBackBufferHeight = 1080;
            GraphicsDeviceManager.IsFullScreen = true;
            GraphicsDeviceManager.ToggleFullScreen();
            GraphicsDeviceManager.ApplyChanges();

            //Player controller
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
            {
                _playerController = new PlayerController();
                _carController = new CarsController();
                await _carController.Setup();
                _gc = new GoalController();
                await _gc.Setup();
            }
            else
            {
                // FAKES!
                _playerController = new PlayerKeyboardController();
                _carController = new TestCarController();
                _gc = new NumberOfQuestionsGoalController(numberOfQuestionsPerRound);
            }

            _questionProvider = new QuestionProvider();

            _scene = new SplashScreen(this, _playerController, _carController, _gc);
            _isInitalized = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Kaller heller egne LoadContents per scene. Kan gjøre det her også, men blir kanskje litt messy?
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_isInitalized)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    GlobalGameState.CurrentState = GameState.Scoreboard;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.B))
                {
                    BackupSender.TrySendBackup();
                }

                if (GlobalGameState.CurrentState != _lastKnownState)
                {
                    _lastKnownState = GlobalGameState.CurrentState;
                    switch (GlobalGameState.CurrentState)
                    {
                        case GameState.SplashScreen:
                            _scene = new SplashScreen(this, _playerController, _carController, _gc);
                            break;
                        case GameState.Scoreboard:
                            _scene = new ScoreboardScene(this, _playerController, _carController, _gc);
                            break;
                        case GameState.PlayerRegistration:
                            GlobalGameState.PlayerOne.Score = 0;
                            GlobalGameState.PlayerTwo.Score = 0;
                            _scene = new NDCPlayerRegistration(this, _playerController, _carController, _gc);
                            break;
                        case GameState.BedExPlayerRegistration:
                            GlobalGameState.PlayerOne.Score = 0;
                            GlobalGameState.PlayerTwo.Score = 0;
                            _scene = new BedExPlayerRegistration(this, _playerController, _carController, _gc);
                            break;
                        case GameState.JavazoneRegistration:
                            GlobalGameState.PlayerOne.Score = 0;
                            GlobalGameState.PlayerTwo.Score = 0;
                            _scene = new JavazoneRegistration(this, _playerController, _carController, _gc);
                            break;
                        case GameState.QuizGame:
                            _scene = new QuizScene(this, _playerController, _questionProvider,_carController, _gc);
                            break;
                        case GameState.GameOver:
                            _scene = new GameOverScene(this, _playerController, _carController, _gc);
                            break;
                        default:
                            throw new Exception($"Du har ikke lagt til riktig scene for {GlobalGameState.CurrentState}.");
                    }
                }

                _scene.Update(gameTime);

                //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Exit();
                }

                base.Update(gameTime);
            }
  
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(DarkPrimary);
            _scene.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
