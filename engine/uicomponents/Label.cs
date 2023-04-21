namespace boreal.engine
{
    public class Label : UIComponent
    {

        public int scale = 1;
        public engine.Color TextColor = engine.Color.White;

        public Font font = FontManager.defaultFont;

        private Microsoft.Xna.Framework.Vector2 textv2;
        private string _text = "Text";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                textv2 = font.font.MeasureString(value);
            }
        }

        internal override Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(
                (int)transform.position.X, (int)transform.position.Y, (int)textv2.X * scale, (int)textv2.Y * scale);
        }

        public Label(string text = null)
        {
            if (text != null)
                this.text = text;
        }

        internal override void DrawUI(Sprites spritesBatch)
        {
            spritesBatch.DrawString(font.font, text, transform.position.xnaV2, TextColor.color, 0, Microsoft.Xna.Framework.Vector2.Zero, scale);

            base.Draw(spritesBatch);
        }

    }
}
