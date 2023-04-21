using System;
using System.IO;

namespace boreal.engine
{
    public class FontManager
    {
        public static string chars = @" !""#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~«»…";

        private static Font[] _fonts = new Font[0];
        public static Font[] fonts
        {
            get { return _fonts; }
            private set { _fonts = value; }
        }

        private static Font _defaultFont = null;
        public static Font defaultFont
        {
            get 
            {
                if (_defaultFont == null)
                {
                    if (File.Exists("assets/arial.ttf"))
                        AddFont(new Font(File.ReadAllBytes("assets/arial.ttf")));
                    else
                        throw new Exception("Unable to load a default font, please use FontManager.AddFont(...) before Launching boreal.");
                }
                return _defaultFont; 
            }
            set { _defaultFont = value; }
        }

        public static void AddFont(Font font)
        {
            fonts = Utils.ResizeStaticArray(fonts, fonts.Length + 1);
            fonts[fonts.Length - 1] = font;
            if(_defaultFont == null) defaultFont = font;
        }

        public static void RemoveFont(Font font)
        {
            fonts = Utils.ResizeStaticArray(fonts, fonts.Length - 1);
        }

    }
}
