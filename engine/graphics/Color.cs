using System;

namespace boreal.engine
{
    public class Color
    {
        internal Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color();
        public byte r { get { return color.R; } set { color.R = value; } }
        public byte g { get { return color.G; } set { color.G = value; } }
        public byte b { get { return color.B; } set { color.B = value; } }
        public byte a { get { return color.A; } set { color.A = value; } }

        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 255;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        internal Color(Microsoft.Xna.Framework.Color color)
        {
            r = color.R;
            g = color.G;
            b = color.B;
            a = color.A;
        }

        public static Color Lerp(Color c1, Color c2, float k)
        {
            var bk = (1 - k);
            var a = c1.a * bk + c2.a * k;
            var r = c1.r * bk + c2.r * k;
            var g = c1.g * bk + c2.g * k;
            var b = c1.b * bk + c2.b * k;
            return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public override string ToString()
        {
            return r + "," + g + "," + b + "," + a;
        }

        public static Color Transparent { get { return ColorReferences[0]; } }
        public static Color White { get { return ColorReferences[1]; } }
        public static Color Gray { get { return ColorReferences[2]; } }
        public static Color Black { get { return ColorReferences[3]; } }
        public static Color Red { get { return ColorReferences[4]; } }
        public static Color Green { get { return ColorReferences[5]; } }
        public static Color Blue { get { return ColorReferences[6]; } }
        public static Color Yellow { get { return ColorReferences[7]; } }
        public static Color Cyan { get { return ColorReferences[8]; } }
        public static Color Magenta { get { return ColorReferences[9]; } }
        public static Color Orange { get { return ColorReferences[10]; } }

        public static Color Random
        {
            get
            {
                Random r = new Random();
                return new Color((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255), 255);
            }
        }

        private static Color[] ColorReferences = new Color[]
        {
            new Color(0,0,0,0),
            new Color(255,255,255,255),
            new Color(128,128,128,255),
            new Color(0,0,0,255),
            new Color(255,0,0,255),
            new Color(0,255,0,255),
            new Color(0,0,255,255),
            new Color(255,255,0,255),
            new Color(0,255,255,255),
            new Color(255,0,255,255),
            new Color(255,128,0,255)
        };

    }
}
