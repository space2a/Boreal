using System.IO;
using System.Linq;

namespace boreal.engine
{
    public class Texture2D
    {
        internal string assetName = "";

        internal Microsoft.Xna.Framework.Graphics.Texture2D texture2D;

        public bool dontDestroy = false;

        public int width
        {
            get { return texture2D.Width; }
        }

        public int height
        {
            get { return texture2D.Height; }
        }

        public Texture2D(string texturePath)
        {
            if (!File.Exists(texturePath))
            {
                var t = LoadEmbeddedResources.LoadTexture("missingtexture.png", "images");
                this.assetName = t.assetName;
                texture2D = t.texture2D;
                return;
            }

            this.assetName = texturePath;
            FileStream fileStream = new FileStream(texturePath, FileMode.Open);
            
            texture2D = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Launcher.core.GraphicsDevice, fileStream);
            fileStream.Dispose();
        }

        public Texture2D(byte[] data)
        {
            texture2D = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Launcher.core.GraphicsDevice, new MemoryStream(data));
        }

        public Texture2D(Stream stream)
        {
            texture2D = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Launcher.core.GraphicsDevice, stream);
        }

        public Texture2D(int width, int height)
        {
            texture2D = new Microsoft.Xna.Framework.Graphics.Texture2D(Launcher.core.GraphicsDevice, width, height);
            texture2D.SetData(Enumerable.Repeat(Microsoft.Xna.Framework.Color.White, width * height).ToArray());
        }

        public Texture2D(int width, int height, Color color)
        {
            texture2D = new Microsoft.Xna.Framework.Graphics.Texture2D(Launcher.core.GraphicsDevice, width, height);
            texture2D.SetData(Enumerable.Repeat(color.color, width * height).ToArray());
        }

        public Texture2D(int width, int height, Color[] colors)
        {
            if (colors.Length != (width * height)) throw new System.Exception("Invalid array size, size expected " + (width * height));
            Microsoft.Xna.Framework.Color[] tColors = new Microsoft.Xna.Framework.Color[colors.Length];

            for (int i = 0; i < colors.Length; i++)
                tColors[i] = colors[i].color;

            texture2D = new Microsoft.Xna.Framework.Graphics.Texture2D(Launcher.core.GraphicsDevice, width, height);
            texture2D.SetData(tColors);
        }

        public bool SetPixel(int x, int y, Color color)
        {
            if (x > texture2D.Width || y > texture2D.Height) return false;
            else if (x < 0 || y < 0) return false;

            if (x == texture2D.Width) x--;
            if (y == texture2D.Height) y--;
            texture2D.SetData(0, new Microsoft.Xna.Framework.Rectangle(x, y, 1, 1), new Microsoft.Xna.Framework.Color[1] { color.color }, 0, 1);
            return true;
        }

        public void SetAllPixels(Color[] colors)
        {
            Microsoft.Xna.Framework.Color[] colorsXNA = new Microsoft.Xna.Framework.Color[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colorsXNA[i] = colors[i].color;
            }

            texture2D.SetData(colorsXNA);
        }

        public Color GetPixel(int x, int y)
        {
            if (x > texture2D.Width || y > texture2D.Height) return Color.Black;
            else if (x < 0 || y < 0) return Color.Black;
            Microsoft.Xna.Framework.Color[] cd = new Microsoft.Xna.Framework.Color[texture2D.Width * texture2D.Height];

            texture2D.GetData<Microsoft.Xna.Framework.Color>(cd);
            return new Color(cd[x + y & texture2D.Width]);
        }

        public Color[] GetAllPixels()
        {
            Microsoft.Xna.Framework.Color[] cd = new Microsoft.Xna.Framework.Color[texture2D.Width * texture2D.Height];
            Color[] c = new Color[texture2D.Width * texture2D.Height];

            texture2D.GetData<Microsoft.Xna.Framework.Color>(cd);

            for (int i = 0; i < cd.Length; i++)
                c[i] = new Color(cd[i]);

            return c;
        }

        public Color[,] GetAllPixels2D()
        {
            var colors = GetAllPixels();

            Color[,] c2d = new Color[width, height];

            int x = 0, y = 0;

            for (int i = 0; i < colors.Length; i++)
            {
                c2d[x, y] = colors[i];
                x++;
                if(x == width)
                {
                    x = 0;
                    y++;
                }
            }

            return c2d;
        }


        public void Dispose()
        {
            if(dontDestroy) return;
            texture2D.Dispose();
        }
    }
}
