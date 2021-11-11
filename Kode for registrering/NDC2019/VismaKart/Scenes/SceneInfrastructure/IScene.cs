using Microsoft.Xna.Framework;

namespace VismaKart.Scenes.SceneInfrastructure
{
    public interface IScene
    {
        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}