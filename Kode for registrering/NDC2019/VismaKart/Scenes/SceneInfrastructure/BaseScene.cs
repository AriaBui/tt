using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VismaKart.Electronics;
using VismaKart.GlobalStuff;

namespace VismaKart.Scenes.SceneInfrastructure
{
    public abstract class BaseScene : IScene
    {
        protected readonly Game Game;
        protected readonly IPlayerController _playerController;
        protected readonly SpriteBatch SpriteBatch;
        protected readonly ICarsController _carsController;
        protected readonly IGoalController _gc;

        protected BaseScene(Game game, IPlayerController playerController, ICarsController carsController, IGoalController goalController)
        {
            Game = game;
            _carsController = carsController;
            _playerController = playerController;
            _gc = goalController;
            SpriteBatch = new SpriteBatch(VismaKart.GraphicsDeviceManager.GraphicsDevice);
            LoadContent();
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            DrawSprites(gameTime);
            SpriteBatch.End();
        }

        public abstract void DrawSprites(GameTime gameTime);

    }
}
