using System;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using Microsoft.IoT.DeviceCore.Pwm;
using Microsoft.IoT.Devices.Pwm;
using Microsoft.IoT.Lightning.Providers;
using Microsoft.Xna.Framework;

namespace VismaKart.Electronics
{
    public class CarsController : ICarsController
    {

        private int car1pwm = 21;
        private int car2pwm = 20;

        private GpioPin Car1;
        private GpioPin Car2;


        public bool car1Direction = true;
        public bool car1Run;
        public TimeSpan car1StartTime;

        public int countCar1 = 0;

        public TimeSpan RunCar1For;
        public TimeSpan RunCar2For;

        public bool car2Direction = true;
        public bool car2Run;
        public TimeSpan car2StartTime;

        public int countCar2 = 0;

        public async Task<bool> Setup()
        {
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }
            var gpio = (await GpioController.GetControllersAsync(LightningGpioProvider.GetGpioProvider()))[0];

            if (gpio == null)
            {
                Console.WriteLine("No GPIO here");
                return false;
            }


            Car1 = gpio.OpenPin(car1pwm);
            Car2 = gpio.OpenPin(car2pwm);
            Car1.Write(GpioPinValue.Low);
            Car2.Write(GpioPinValue.Low);
            return true;
        }

        public void Car1Run()
        {
            Car1.Write(GpioPinValue.High);
        }

        public void StopCar1()
        {
            Car1.Write(GpioPinValue.Low);
            car1Run = false;
        }

        public void StopCar2()
        {
            Car2.Write(GpioPinValue.Low);
            car2Run = false;
        }


        public void Car2Run()
        {
            Car2.Write(GpioPinValue.High);
        }

        public void UpdateCars(GameTime gameTime)
        {
            //Run cars
            if (car1Run)
            {
                Car1Run();
                if (gameTime.TotalGameTime >= car1StartTime.Add(RunCar1For))
                {
                    StopCar1();
                }
            }
            if (car2Run)
            {
                Car2Run();
                if (gameTime.TotalGameTime >= car2StartTime.Add(RunCar2For))
                {
                    StopCar2();
                }
            }
        }

        public void ToggleRunCar1(GameTime time, TimeSpan runFor)
        {
            car1Run = true;
            car1StartTime = time.TotalGameTime;
            RunCar1For = runFor;

        }

        public void ToggleRunCar2(GameTime gameTime, TimeSpan runFor)
        {
            car2Run = true;
            car2StartTime = gameTime.TotalGameTime;
            RunCar2For = runFor;
        }

    }
}