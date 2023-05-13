namespace boreal.engine
{
    public class Button : UIComponent
    {
        public Texture2D buttonTexture = LoadEmbeddedResources.LoadTexture("button.png", "images");
        public Texture2D buttonHoveringTexture = null;

        public Texture2D buttonIcon = null;

        public Color backgroundColor = Color.Gray;
        public bool useShape = false;
        public Font font = FontManager.defaultFont;

        private Microsoft.Xna.Framework.Vector2 textv2;
        private string _text = "Button";
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

        public Color textColor = Color.White;
        public Color hoveringColor = Color.Gray;

        public bool resizeTextureToTextLength = true;

        public Button()
        {
        }

        public Button(string text)
        {
            this.text = text;
        }


        public Button(Texture2D buttonTexture, bool resizeTextureToTextLength = true)
        {
            this.buttonTexture = buttonTexture;
            this.resizeTextureToTextLength = resizeTextureToTextLength;
        }

        internal override void DrawUI(Drawer spritesBatch)
        {
            var elementRectangle = ElementRectangle;

            var color = Color.White;
            if (useShape) color = backgroundColor;

            if (isHovering) color = hoveringColor;

            if (!useShape)
            {
                if (isHovering)
                {
                    if (buttonHoveringTexture != null)
                        spritesBatch.Draw(buttonHoveringTexture.texture2D, ElementRectangle, color.color);
                    else
                        spritesBatch.Draw(buttonTexture.texture2D, ElementRectangle, color.color);
                }
                else spritesBatch.Draw(buttonTexture.texture2D, ElementRectangle, color.color);
            }
            else
                spritesBatch.shapes.DrawFilledRectangle(elementRectangle, color);

            float x = elementRectangle.X + elementRectangle.Width / 2 - textv2.X / 2;
            float y = elementRectangle.Y + elementRectangle.Height / 2 - textv2.Y / 2;

            spritesBatch.DrawString(font.font, text, new Microsoft.Xna.Framework.Vector2(x, y), textColor.color);

            spritesBatch.shapes.DrawRectangleOutline(elementRectangle, new Color(255, 0, 255, 100), 10);
        }

        internal override Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            if (resizeTextureToTextLength)
            {
                return new Microsoft.Xna.Framework.Rectangle((int)transform.position.X,
                (int)transform.position.Y,
                buttonTexture.texture2D.Width + (int)textv2.X, buttonTexture.texture2D.Height);
            }
            else
            {
                return new Microsoft.Xna.Framework.Rectangle((int)transform.position.X,
                (int)transform.position.Y,
                buttonTexture.texture2D.Width, buttonTexture.texture2D.Height);
            }
        }

        internal override void DestroyComponent()
        {
            buttonTexture.Dispose();
        }

    }
}
