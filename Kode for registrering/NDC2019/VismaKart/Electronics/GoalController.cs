using System;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Devices.Gpio;
using Microsoft.IoT.Lightning.Providers;

namespace VismaKart.Electronics
{
    public class GoalController : IGoalController
    {
        private int p1 = 16;
        private int p2 = 26;

        private GpioPin P1;
        private GpioPin P2;

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

            P1 = gpio.OpenPin(p1);
            P2 = gpio.OpenPin(p2);


            P1.SetDriveMode(GpioPinDriveMode.Input);
            P2.SetDriveMode(GpioPinDriveMode.Input);

            return true;
        }

        public bool Player1IsInGoal()
        {
            if (P1.Read() == GpioPinValue.High)
                return true;
            return false;
        }

        public bool Player2IsInGoal()
        {
            if (P2.Read() == GpioPinValue.High)
                return true;
            return false;
        }
    }
}