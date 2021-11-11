using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VismaKart.GlobalStuff
{
    public class Speedometer
    {
        private SpriteBatch spriteBatch;

        private const float MAX_METER_ANGLE = 260;

        private float scale;
        private float lastAngle;

        private Vector2 meterPosition;
        private Vector2 meterOrigin;

        private Texture2D backgroundImage;
        private Texture2D needleImage;

        public float currentAngle = 0;

        public Speedometer(Vector2 position, Texture2D backgroundImage, Texture2D needleImage, SpriteBatch spriteBatch, float scale)
        {
            this.spriteBatch = spriteBatch;

            this.backgroundImage = backgroundImage;
            this.needleImage = needleImage;
            this.scale = scale;

            this.lastAngle = -89;

            meterPosition = position;
            meterOrigin = new Vector2(40, 120);
        }
        public void Update(float currentValue, float maximumValue)
        {
            currentAngle = MathHelper.SmoothStep(lastAngle, (currentValue / maximumValue) * MAX_METER_ANGLE, 0.2f) - 89;
            lastAngle = currentAngle;
        }


        public void Draw()
        {
            spriteBatch.Draw(backgroundImage, meterPosition, null, Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            spriteBatch.Draw(needleImage, meterPosition + new Vector2(75, 75), null, Color.White, MathHelper.ToRadians(currentAngle), meterOrigin, scale, SpriteEffects.None, 0);
        }
    }
}
