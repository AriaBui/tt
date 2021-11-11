using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VismaKart.GlobalStuff;

namespace VismaKart.Electronics
{
    class PlayerKeyboardController : IPlayerController
    {
        private bool _isKeyDownA = false;
        private bool _isKeyDownS = false;
        private bool _isKeyDownD = false;
        private bool _isKeyDownF = false;

        private bool _isKeyDownH = false;
        private bool _isKeyDownJ = false;
        private bool _isKeyDownK = false;
        private bool _isKeyDownL = false;

        public Button GetPlayer1Input()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.A))
            {
                _isKeyDownA = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.S))
            {
                _isKeyDownS = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.D))
            {
                _isKeyDownD = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.F))
            {
                _isKeyDownF = false;
            }

            if (!_isKeyDownA && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _isKeyDownA = true;
                return Button.Red;
            }
            if (!_isKeyDownS && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _isKeyDownS = true;
                return Button.Yellow;
            }
            if (!_isKeyDownD && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _isKeyDownD = true;
                return Button.Green;
            }
            if (!_isKeyDownF && Keyboard.GetState().IsKeyDown(Keys.F))
            {
                _isKeyDownF = true;
                return Button.Blue;
            }
            
            return Button.None;
        }

        public Button GetPlayer2Input()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.H))
            {
                _isKeyDownH = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.J))
            {
                _isKeyDownJ = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.K))
            {
                _isKeyDownK = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.L))
            {
                _isKeyDownL = false;
            }

            if (!_isKeyDownH && Keyboard.GetState().IsKeyDown(Keys.H))
            {
                _isKeyDownH = true;
                return Button.Red;
            }
            if (!_isKeyDownJ && Keyboard.GetState().IsKeyDown(Keys.J))
            {
                _isKeyDownJ = true;
                return Button.Yellow;
            }
            if (!_isKeyDownK && Keyboard.GetState().IsKeyDown(Keys.K))
            {
                _isKeyDownK = true;
                return Button.Green;
            }
            if (!_isKeyDownL && Keyboard.GetState().IsKeyDown(Keys.L))
            {
                _isKeyDownL = true;
                return Button.Blue;
            }

            return Button.None;
        }
    }
}
