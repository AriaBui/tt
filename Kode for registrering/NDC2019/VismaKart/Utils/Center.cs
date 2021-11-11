using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VismaKart.GlobalStuff;

namespace VismaKart.Utils
{
    public static class Center
    {
        public static Vector2 GetVectorPositionFromCenter(Game Game, int FromCenterX, int fromCenterY)
        {
            return new Vector2(Game.GraphicsDevice.Viewport.Width / 2f + FromCenterX, Game.GraphicsDevice.Viewport.Height / 2f + fromCenterY);
        }

        public static Vector2 GetVectorPositionFromCenter(Game Game, float FromCenterX, float fromCenterY)
        {
            return new Vector2((int)(Game.GraphicsDevice.Viewport.Width / 2f + FromCenterX), (int)(Game.GraphicsDevice.Viewport.Height / 2f + fromCenterY));
        }

        public static Rectangle GetRectanglePositionFromCenter(Game Game, int FromCenterX, int fromCenterY, int width, int height)
        {
            return new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 + FromCenterX, Game.GraphicsDevice.Viewport.Height / 2 + fromCenterY, width, height);
        }

        public static Rectangle GetRectanglePositionFromCenter(Game Game, float FromCenterX, float fromCenterY, float width, float height)
        {
            return new Rectangle((int)(Game.GraphicsDevice.Viewport.Width / 2f + FromCenterX), (int)(Game.GraphicsDevice.Viewport.Height / 2f + fromCenterY), (int)width, (int)height);
        }

        public static Vector2 GetHorizontallyCenteredVectorWithText(Game Game, int FromCenterX, int fromCenterY, int width, SpriteFont font, string text)
        {
            var textLength = font.MeasureString(text).X;

            return new Vector2(Game.GraphicsDevice.Viewport.Width / 2 + FromCenterX + (width - textLength) / 2f, Game.GraphicsDevice.Viewport.Height / 2 + fromCenterY);
        }


        public static Vector2 GetHorizontallyAndVerticallyCenteredVectorWithTextWrap(Game Game, int FromCenterX, int fromCenterY, int width, int height, SpriteFont font, string text)
        {
            var wrappedText = WordWrap.WrapText(font, text, width);
            var measuredStringVector = font.MeasureString(wrappedText);
            var textLength = measuredStringVector.X;
            var textHeight = measuredStringVector.Y;

            return new Vector2(Game.GraphicsDevice.Viewport.Width / 2 + FromCenterX + (width - textLength) / 2f, Game.GraphicsDevice.Viewport.Height / 2 + fromCenterY + (height - textHeight) / 2f);
        }
    }
}
