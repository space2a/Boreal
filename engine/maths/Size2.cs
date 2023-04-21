namespace boreal.engine
{
    public class Size2
    {
        public int width, height;
        public Size2(int w, int h) 
        { width = w; height = h; }

        public Size2(Vector2 vector2)
        {
            width = (int)vector2.X;
            height = (int)vector2.Y;
        }

        public static Size2 FromTexture2D(Texture2D texture2D)
        {
            if (texture2D != null)
            {
                return new Size2(texture2D.texture2D.Width, texture2D.texture2D.Height);
            }
            else return new Size2(0, 0);
        }

        public override string ToString()
        {
            return width + ";" + height;
        }
    }
}
