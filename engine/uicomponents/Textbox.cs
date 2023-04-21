using Microsoft.Xna.Framework;

using System;

namespace boreal.engine
{
    public class Textbox : UIComponent
    {
        public delegate void OnTextUpdate(string text);
        public event OnTextUpdate OnTextChanged;

        public TextboxType textboxType { get; private set; }

        public Color backgroundColor = Color.White;
        public Texture2D backgroundTexture = LoadEmbeddedResources.LoadTexture("textbox.png", "images");

        public bool centerText = false;

        public bool isReadOnly = false;
        public bool isPasswordField = false;

        public char passwordChar = '*';

        public int maxLength = 2048;

        public Color textColor = Color.Black;

        public bool useShape = false;

        private int _caretIndex = 0;
        public int caretIndex
        {
            get { return _caretIndex; }
            set { _caretIndex = value; caretv2 = font.font.MeasureString(_text.Substring(0, MathU.Clamp(value, 0, _text.Length))); }
        }

        public int caretWidth = 2;
        public Color caretColor = new Color(255, 255, 255, 128);
        public bool showCaret = true;

        public Color selectionColor = new Color(41, 116, 214, 100);

        public bool isCaretBlinking
        {
            get { return _isCaretBlinking; }
            set { _isCaretBlinking = value; if (!value) drawCaret = true; }
        }

        public Font font = FontManager.defaultFont;

        public bool isSelecting { get; private set; }

        private Microsoft.Xna.Framework.Vector2 textv2;
        private Microsoft.Xna.Framework.Vector2 caretv2;
        private string _text = "Textbox";

        private bool drawCaret = true;
        private bool _isCaretBlinking = true;

        private int startSelectionIndex = 0;
        private Microsoft.Xna.Framework.Vector2 startSelectionIndexv2;

        private bool isHoldingShift = false;

        public string text
        {
            get
            {
                return _text;
            }
            private set
            {
                _text = value;
                OnTextChanged?.Invoke(value);
                caretv2 = font.font.MeasureString(_text.Substring(0, caretIndex));
                if (string.IsNullOrWhiteSpace(value))
                {
                    textv2 = font.font.MeasureString("xxx"); //y should never be 0
                }
                else textv2 = font.font.MeasureString(value);
            }
        }

        private Size2 baseSize = new Size2(200, 40);

        public Textbox()
        {
            textboxType = TextboxType.Regular;
            Initialize();
        }

        public Textbox(bool isReadOnly)
        {
            textboxType = TextboxType.Regular;
            this.isReadOnly = isReadOnly;
            showCaret = !isReadOnly;

            Initialize();
        }

        public Textbox(TextboxType textboxType)
        {
            this.textboxType = textboxType;
            Initialize();
        }

        private void Initialize()
        {
            calculHovering = true;

            scissorsElementRectangle = true;

            Launcher.core.Window.TextInput += Window_TextInput;
            Launcher.core.Window.KeyDown += Window_KeyDown;

            LeftClick += Textbox_LeftClick;
            LeftClickHold += Textbox_LeftClickHold;
            LeftClickReleased += Textbox_LeftClickReleased;

            caretIndex = text.Length;
            text = text; //update textv2 and caretv2
        }

        public void SetText(string text, bool moveCaret = true)
        {
            this.text = text;
            if (moveCaret) caretIndex = text.Length;
        }

        public void Select(int from, int to)
        {
            caretIndex = to;

            startSelectionIndex = from;
            startSelectionIndexv2 = font.font.MeasureString(_text.Substring(0, startSelectionIndex));

            isSelecting = true;
            isHoldingShift = true;
        }

        private void Window_KeyDown(object sender, InputKeyEventArgs e)
        {
            ProcessKey(e.Key);
        }

        private bool ignoreKey = false;
        private void ProcessKey(Microsoft.Xna.Framework.Input.Keys key)
        {
            switch (key)
            {
                case (Microsoft.Xna.Framework.Input.Keys)Keys.Back:
                    ignoreKey = true;
                    if (text.Length > 0 && caretIndex > 0 && !isReadOnly)
                    {
                        //Console.WriteLine(caretIndex + " => " + (caretIndex - 1) + " | " + text.Length + " === " + text);
                        caretIndex -= 1;
                        text = _text.Remove(caretIndex, 1);
                    }
                    break;

                case (Microsoft.Xna.Framework.Input.Keys)Keys.Delete:
                    ignoreKey = true;
                    if (text.Length > 0 && caretIndex > 0 && caretIndex + 1 <= text.Length && !isReadOnly)
                    {
                        text = _text.Remove(caretIndex, 1);
                    }
                    break;

                case (Microsoft.Xna.Framework.Input.Keys)Keys.Left:
                    if (caretIndex > 0) { caretIndex--; drawCaret = true; }
                    break;

                case (Microsoft.Xna.Framework.Input.Keys)Keys.Right:
                    if (caretIndex < text.Length) { caretIndex++; drawCaret = true; }
                    break;

            }
            if (!isHoldingShift) isSelecting = false;
        }

        private void Window_TextInput(object sender, TextInputEventArgs keyarg)
        {
            if (CanTypeThis(keyarg) && !ignoreKey)
            {
                drawCaret = true;
                caretIndex++;
                ModifyText(keyarg.Character.ToString());
            }

            ignoreKey = false;
        }

        private void ModifyText(string t)
        {
            text = text.Insert(caretIndex - 1, t);
            Selection(false);
            return;
            if (isSelecting && startSelectionIndex - caretIndex != -1)
            {
                //replace the selection with the text variable.
                string selected = "";
                Console.WriteLine("replacing...");
                if (caretIndex > startSelectionIndex)
                {
                    Console.WriteLine("rep>");
                    selected = _text.Substring(startSelectionIndex, caretIndex - 1);
                }
                else if (caretIndex < startSelectionIndex)
                {
                    Console.WriteLine("rep<");
                    selected = _text.Substring(caretIndex, startSelectionIndex - 1);
                }
                Console.WriteLine(selected);
            }
            else
            {
                text = text.Insert(caretIndex - 1, t);
                Selection(false);
            }
        }

        private bool CanTypeThis(TextInputEventArgs keyarg)
        {
            if (!isSelected || isReadOnly) return false;
            if (text.Length >= maxLength) return false;

            if (textboxType == TextboxType.Regular) return true;
            else if (textboxType == TextboxType.LettersOnly) return char.IsLetter(keyarg.Character);
            else if (textboxType == TextboxType.NumbersOnly) return char.IsDigit(keyarg.Character);
            return false;
        }

        internal override void DrawUI(Sprites spritesBatch)
        {
            var elementRectangle = ElementRectangle;

            //spritesBatch.core.GraphicsDevice.ScissorRectangle = elementRectangle;

            if (useShape)
                spritesBatch.shapes.DrawFilledRectangle(elementRectangle, backgroundColor);
            else
                spritesBatch.Draw(backgroundTexture.texture2D, ElementRectangle, Microsoft.Xna.Framework.Color.White);

            float y = elementRectangle.Y + elementRectangle.Height / 2 - textv2.Y / 2;
            if (centerText)
            {
                float x = elementRectangle.X + elementRectangle.Width / 2 - textv2.X / 2;
                elementRectangle.X = (int)x;
            }

            spritesBatch.DrawString(font.font, DrawText(),
                new Microsoft.Xna.Framework.Vector2(elementRectangle.X, y),
                textColor.color); //need scaling

            //caret
            if (drawCaret && showCaret && isSelected)
            {
                spritesBatch.shapes.DrawFilledRectangle(
                new Microsoft.Xna.Framework.Rectangle(elementRectangle.X + (int)caretv2.X,
                (int)y, caretWidth, (int)(elementRectangle.Height / 1.5)), caretColor);
            }

            //selection
            if (isSelecting)
            {
                Microsoft.Xna.Framework.Rectangle selectionRectangle = new Microsoft.Xna.Framework.Rectangle();
                if (caretIndex > startSelectionIndex)
                {
                    selectionRectangle = new Microsoft.Xna.Framework.Rectangle(
                    elementRectangle.X + (int)startSelectionIndexv2.X,
                    (int)y,
                    (int)(caretv2.X - startSelectionIndexv2.X),
                    (int)(elementRectangle.Height / 1.5));
                }

                else if (caretIndex < startSelectionIndex)
                {
                    selectionRectangle = new Microsoft.Xna.Framework.Rectangle(
                    elementRectangle.X + (int)caretv2.X,
                    (int)y,
                    (int)(startSelectionIndexv2.X - caretv2.X),
                    (int)(elementRectangle.Height / 1.5));
                }

                spritesBatch.shapes.DrawFilledRectangle(
                selectionRectangle, selectionColor);
            }
        }

        private string DrawText()
        {
            string outputText = text;

            if (isPasswordField)
            {
                outputText = "";
                for (int i = 0; i < text.Length; i++)
                {
                    outputText += passwordChar;
                }
            }

            return outputText;
        }

        private void MoveCaretToCursor()
        {
            var elementRectangle = ElementRectangle;

            float y = elementRectangle.Y + elementRectangle.Height / 2 - textv2.Y / 2;

            if (centerText)
            {
                float cX = elementRectangle.X + elementRectangle.Width / 2 - textv2.X / 2;
                elementRectangle.X = (int)cX;
            }

            int x = 0;

            for (int i = 0; i < text.Length; i++)
            {
                var v2 = font.GetLetterVector2(text[i]);
                Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(
                    elementRectangle.X + x,
                    (int)y,
                    (int)v2.X / 2,
                    (int)v2.Y);

                x += (int)v2.X / 2;

                Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(
                elementRectangle.X + x,
                (int)y,
                (int)v2.X / 2,
                (int)v2.Y);

                x += (int)v2.X / 2;

                Microsoft.Xna.Framework.Rectangle mouseRectangle = new Microsoft.Xna.Framework.Rectangle();
                if (gameObject.isInCanvas) mouseRectangle = windowMouseRectangle;
                else mouseRectangle = worldMouseRectangle;

                if (mouseRectangle.Intersects(rectangle))
                {
                    caretIndex = i;
                    return;
                }
                else if (mouseRectangle.Intersects(rectangle2))
                {
                    caretIndex = i + 1;
                    return;
                }

            }
        }

        private void Textbox_LeftClick(object sender, EventArgs e)
        {
            MoveCaretToCursor();
            if (!isHoldingShift)
            {
                startSelectionIndex = caretIndex;
                startSelectionIndexv2 = font.font.MeasureString(_text.Substring(0, startSelectionIndex));
            }

        }

        int holdCycles = 0;
        private void Textbox_LeftClickHold(object sender, EventArgs e)
        {
            holdCycles++;
            if (holdCycles > 10)
            {
                isSelecting = true;
                MoveCaretToCursor();
            }
        }

        private void Textbox_LeftClickReleased(object sender, EventArgs e)
        {
            holdCycles = 0;
        }

        private float caretInterval = 0;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!isSelected) return;
            if (showCaret && isCaretBlinking)
            {
                if ((caretInterval += 1 * gameTime.ElapsedTotalSeconds) >= 0.65f) { caretInterval = 0; drawCaret = !drawCaret; }
            }
            if (Inputs.KeyboardState.GetKeyDown(Keys.LeftShift))
            {
                isHoldingShift = true;
                Selection();
            }
            else if (Inputs.KeyboardState.GetKeyUp(Keys.LeftShift)) isHoldingShift = false;

            if (Inputs.KeyboardState.GetKeyDown(Keys.LeftControl) && Inputs.KeyboardState.GetKey(Keys.A)) Select(0, text.Length);
        }

        private void Selection(bool selecting = true)
        {
            startSelectionIndex = caretIndex;
            startSelectionIndexv2 = font.font.MeasureString(_text.Substring(0, startSelectionIndex));
            isSelecting = selecting;
        }


        internal override Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(
                (int)transform.position.X,
                (int)transform.position.Y,
                baseSize.width * (int)transform.scale.X,
                baseSize.height * (int)transform.scale.Y);
        }

        public enum TextboxType
        {
            Regular,
            LettersOnly,
            NumbersOnly
        }

    }
}