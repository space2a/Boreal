using Microsoft.Xna.Framework.Graphics;

using SpriteFontPlus;

using System;
using System.IO;

namespace boreal.engine
{
    public class Font
    {
        public Vector2[] charsv2 { get; private set; }

        internal SpriteFont font;

        public Font(string fontPath)
        {
            CreateFont(File.ReadAllBytes(fontPath));
        }

        public Font(byte[] fontData)
        {
            CreateFont(fontData);
        }

        private void CreateFont(byte[] data)
        {
            var fontBakeResult = TtfFontBaker.Bake(data,
                25,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );

            font = fontBakeResult.CreateSpriteFont(Launcher.core.GraphicsDevice);
            CalculateV2s();
        }

        public Vector2 GetLetterVector2(char c, bool removeDiacritics = true)
        {
            string t = c.ToString();
            if (removeDiacritics) t = RemoveDiacritics(t);

            int index = FontManager.chars.IndexOf(t);
            if (index == -1) { FontManager.chars += t; CalculateV2s(); return GetLetterVector2(c); }
            else return charsv2[index];
        }


        private string RemoveDiacritics(string text)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(text);
            return System.Text.Encoding.UTF8.GetString(tempBytes);
        }

        public void CalculateV2s()
        {
            charsv2 = new Vector2[FontManager.chars.Length];

            DateTime start = DateTime.Now;
            for (int i = 0; i < charsv2.Length; i++)
            {
                charsv2[i] = new Vector2(font.MeasureString(FontManager.chars[i].ToString()));
            }

            Console.WriteLine("finished " + charsv2.Length + " duration:" + (DateTime.Now - start).TotalSeconds);
        }


    }
}
