namespace boreal.engine.graphics
{
    public class EssentialDrawer
    {
        internal Drawer drawer;
        public EssentialDrawer() { }

        public void DrawTexture2D(Texture2D texture2D, Color color, Vector2 position, Vector2 origin, int orderBy = 0)
        {
            drawer.Draw(texture2D.texture2D, origin.xnaV2, position.xnaV2, color, orderBy: orderBy);
        }

        public void DrawString(string text, Color color, Vector2 position, float scale = 1, Font font = null, int orderBy = 0)
        {
            if (font == null) font = FontManager.defaultFont;
            drawer.DrawString(font.font, text, position.xnaV2, color.color, scale: scale, orderBy: orderBy);
        }

    }

}
