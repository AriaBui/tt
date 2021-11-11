using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VismaKart.Electronics
{
    class TestCarController : ICarsController
    {
        public async Task<bool> Setup()
        {
            await Task.CompletedTask;
            return false;
        }

        public void Car1Run()
        {
        }

        public void StopCar1()
        {
        }

        public void StopCar2()
        {
        }

        public void Car2Run()
        {
        }

        public void UpdateCars(GameTime gameTime)
        {
        }

        public void ToggleRunCar1(GameTime gameTime, TimeSpan runFor)
        {
        }

        public void ToggleRunCar2(GameTime gameTime, TimeSpan runFor)
        {
        }
    }
}
