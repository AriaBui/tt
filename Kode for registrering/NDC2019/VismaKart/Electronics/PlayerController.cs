using System;
using Windows.Devices;
using Windows.Devices.Gpio;
using Microsoft.IoT.Lightning.Providers;

namespace VismaKart.Electronics
{
    public class PlayerController : IPlayerController
    {
        private int p1Red = 25;
        private int p1Yellow = 8;
        private int p1Green = 9;
        private int p1Blue = 11;

        private int p2Red = 23;
        private int p2Yellow = 10;
        private int p2Green = 22;
        private int p2Blue = 24;

        private GpioPin p1b;
        private GpioPin p1y;
        private GpioPin p1r;
        private GpioPin p1g;
        private GpioPin p2b;
        private GpioPin p2y;
        private GpioPin p2r;
        private GpioPin p2g;

        private bool _isKeyDownP1Green = false;
        private bool _isKeyDownP1Red = false;
        private bool _isKeyDownP1Blue = false;
        private bool _isKeyDownP1Yellow = false;

        private bool _isKeyDownP2Green = false;
        private bool _isKeyDownP2Red = false;
        private bool _isKeyDownP2Blue = false;
        private bool _isKeyDownP2Yellow = false;

        public PlayerController()
        {
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
            }

            //var gpio = GpioController.GetDefaultAsync().GetAwaiter().GetResult();

            var gpio = GpioController.GetControllersAsync(LightningGpioProvider.GetGpioProvider())
                .GetAwaiter().GetResult()[0];

            if (gpio == null)
            {
                Console.WriteLine("No GPIO here");
                return;
            }

            p1b = gpio.OpenPin(p1Blue);
            p1y = gpio.OpenPin(p1Yellow);
            p1r = gpio.OpenPin(p1Red);
            p1g = gpio.OpenPin(p1Green);

            p2b = gpio.OpenPin(p2Blue);
            p2y = gpio.OpenPin(p2Yellow);
            p2r = gpio.OpenPin(p2Red);
            p2g = gpio.OpenPin(p2Green);

            p1b.Write(GpioPinValue.Low);
            p1b.SetDriveMode(GpioPinDriveMode.Input);
            p1y.Write(GpioPinValue.Low);
            p1y.SetDriveMode(GpioPinDriveMode.Input);
            p1r.Write(GpioPinValue.Low);
            p1r.SetDriveMode(GpioPinDriveMode.Input);
            p1g.Write(GpioPinValue.Low);
            p1g.SetDriveMode(GpioPinDriveMode.Input);

            p2b.Write(GpioPinValue.Low);
            p2b.SetDriveMode(GpioPinDriveMode.Input);
            p2y.Write(GpioPinValue.Low);
            p2y.SetDriveMode(GpioPinDriveMode.Input);
            p2r.Write(GpioPinValue.Low);
            p2r.SetDriveMode(GpioPinDriveMode.Input);
            p2g.Write(GpioPinValue.Low);
            p2g.SetDriveMode(GpioPinDriveMode.Input);


            //https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/IoT-GPIO/cs/Scenario1_GetAndSetPin.xaml.cs
        }

        public Button GetPlayer1Input()
        {
            if (p1g.Read() == GpioPinValue.Low)
            {
                _isKeyDownP1Green = false;
            }
            if (p1r.Read() == GpioPinValue.Low)
            {
                _isKeyDownP1Red = false;
            }
            if (p1b.Read() == GpioPinValue.Low)
            {
                _isKeyDownP1Blue = false;
            }
            if (p1y.Read() == GpioPinValue.Low)
            {
                _isKeyDownP1Yellow = false;
            }

            if (!_isKeyDownP1Green && p1g.Read() == GpioPinValue.High)
            {
                _isKeyDownP1Green = true;
                return Button.Green;
            }
            if (!_isKeyDownP1Red && p1r.Read() == GpioPinValue.High)
            {
                _isKeyDownP1Red = true;
                return Button.Red;
            }
            if (!_isKeyDownP1Blue && p1b.Read() == GpioPinValue.High)
            {
                _isKeyDownP1Blue = true;
                return Button.Blue;
            }
            if (!_isKeyDownP1Yellow && p1y.Read() == GpioPinValue.High)
            {
                _isKeyDownP1Yellow = true;
                return Button.Yellow;
            }

            return Button.None;
        }

        public Button GetPlayer2Input()
        {
            if (p2g.Read() == GpioPinValue.Low)
            {
                _isKeyDownP2Green = false;
            }
            if (p2r.Read() == GpioPinValue.Low)
            {
                _isKeyDownP2Red = false;
            }
            if (p2b.Read() == GpioPinValue.Low)
            {
                _isKeyDownP2Blue = false;
            }
            if (p2y.Read() == GpioPinValue.Low)
            {
                _isKeyDownP2Yellow = false;
            }

            if (!_isKeyDownP2Green && p2g.Read() == GpioPinValue.High)
            {
                _isKeyDownP2Green = true;
                return Button.Green;
            }
            if (!_isKeyDownP2Red && p2r.Read() == GpioPinValue.High)
            {
                _isKeyDownP2Red = true;
                return Button.Red;
            }
            if (!_isKeyDownP2Blue && p2b.Read() == GpioPinValue.High)
            {
                _isKeyDownP2Blue = true;
                return Button.Blue;
            }
            if (!_isKeyDownP2Yellow && p2y.Read() == GpioPinValue.High)
            {
                _isKeyDownP2Yellow = true;
                return Button.Yellow;
            }

            return Button.None;
        }
    }
}