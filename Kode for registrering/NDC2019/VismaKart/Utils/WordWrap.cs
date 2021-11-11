using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace VismaKart.Utils
{
    public static class WordWrap
    {
        public static string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            var words = text.Split(' ');
            var sb = new StringBuilder();
            var lineWidth = 0f;
            var spaceWidth = font.MeasureString(" ").X;

            foreach (var word in words)
            {
                var q = font.MeasureString(word);

                if (lineWidth + q.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += q.X + spaceWidth;
                }
                else
                {
                    if (q.X > maxLineWidth)
                    {
                        if (sb.ToString() == "")
                        {
                            sb.Append(WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                        else
                        {
                            sb.Append("\n" + WrapText(font, word.Insert(word.Length / 2, " ") + " ", maxLineWidth));
                        }
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = q.X + spaceWidth;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
