using Microsoft.Xna.Framework.Input;

using System;

namespace boreal.engine
{
    public class Checkbox : UIComponent
    {
        public event EventHandler isCheckedChanged;

        private Microsoft.Xna.Framework.Input.MouseState currentMouseState;
        private Microsoft.Xna.Framework.Input.MouseState previousMouseState;

        public Texture2D uncheckedTexture = LoadEmbeddedResources.LoadTexture("uncheckedbox.png", "images");
        public Texture2D checkedTexture = LoadEmbeddedResources.LoadTexture("checkedbox.png", "images");

        public engine.Color TextColor = engine.Color.White;
        public engine.Color HoveringTint = engine.Color.Gray;

        public bool isChecked = false;
        public int spacing = 10;

        public bool extendInteractableZoneToText = false;

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

        public Texture2D currentTexture
        {
            get
            {
                if (isChecked) return checkedTexture;
                else return uncheckedTexture;
            }
        }

        public Checkbox()
        {

        }

        public Checkbox(Texture2D uncheckedTexture, Texture2D checkedTexture)
        {
            this.uncheckedTexture = uncheckedTexture;
            this.checkedTexture = checkedTexture;
        }

        internal override Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle((int)transform.position.X,
                (int)transform.position.Y,
                currentTexture.texture2D.Width + (extendInteractableZoneToText == true ? spacing + (int)textv2.X : 0), currentTexture.texture2D.Height);
        }


        internal override void DrawUI(Sprites spritesBatch)
        {
            var elementRectangle = ElementRectangle;
            var color = Microsoft.Xna.Framework.Color.White;

            if (isHovering) color = HoveringTint.color;

            spritesBatch.Draw(currentTexture.texture2D,
                new Microsoft.Xna.Framework.Rectangle(elementRectangle.X, elementRectangle.Y, elementRectangle.Width - (extendInteractableZoneToText == true ? spacing + (int)textv2.X : 0), elementRectangle.Height),
                color);

            var x = elementRectangle.X + currentTexture.texture2D.Width + spacing;
            var y = elementRectangle.Y + elementRectangle.Height / 2 - textv2.Y / 2;

            spritesBatch.DrawString(font.font, text, new Microsoft.Xna.Framework.Vector2(x, y), TextColor.color);

            base.Draw(spritesBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (isHovering && currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                isChecked = !isChecked;
                isCheckedChanged?.Invoke(this, new EventArgs());
            }
            base.Update(gameTime);
        }

        internal override void DestroyComponent()
        {
            uncheckedTexture.Dispose();
            checkedTexture.Dispose();
        }

    }
}
