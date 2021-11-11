using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VismaKart.Electronics
{
    public interface ICarsController
    {
        Task<bool> Setup();
        void Car1Run();
        void StopCar1();
        void StopCar2();
        void Car2Run();

        void UpdateCars(GameTime gameTime);

        void ToggleRunCar1(GameTime gameTime, TimeSpan runFor);
        void ToggleRunCar2(GameTime gameTime, TimeSpan runFor);
    }
}