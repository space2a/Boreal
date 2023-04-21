using MonoGame.Extended;

using Newtonsoft.Json.Linq;

namespace boreal.engine
{
    public class Rectangle
    {

        internal Microsoft.Xna.Framework.Rectangle rect;

        public int X
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        public int Y
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }

        public int Width
        {
            get { return rect.Width; }
            set { rect.Width = value; }
        }
        public int Height
        {
            get { return rect.Height; }
            set { rect.Height = value; }
        }

        public int Left => X;

        public int Right => X + Width;

        public int Top => Y;

        public int Bottom => Y + Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal Rectangle(Microsoft.Xna.Framework.Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        internal Rectangle(RectangleF rectangle)
        {
            X = (int)rectangle.Left;
            Y = (int)rectangle.Top;
            Width = (int)rectangle.Width;
            Height = (int)rectangle.Height;
        }

        public bool Intersects(Rectangle rectangle)
        {
            if (rectangle.Left < Right && Left < rectangle.Right && rectangle.Top < Bottom)
            {
                return Top < rectangle.Bottom;
            }
            return false;
        }

        public int XYWH()
        {
            return X + Y + Width + Height;
        }

        public override string ToString()
        {
            return X + "," + Y + ";" + Width + "," + Height;
        }
    }
}
