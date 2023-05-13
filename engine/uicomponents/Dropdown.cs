using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public class Dropdown : UIComponent
    {
        public delegate void OnSelectionChange(string text);
        public event OnSelectionChange OnSelectionChanged;

        private List<DropdownItem> _items;

        public List<DropdownItem> items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                listv2 = new Microsoft.Xna.Framework.Vector2[value.Count];
                for (int i = 0; i < value.Count; i++)
                {
                    listv2[i] = textFont.font.MeasureString(value[i].text);
                }
            }
        }

        private Microsoft.Xna.Framework.Vector2[] listv2;

        private DropdownItem _selectedItem = new DropdownItem();
        public DropdownItem selectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                textv2 = textFont.font.MeasureString(value.text);
            }
        }

        public int selectedItemIndex = -1;

        public bool isOpen = false;

        public Font textFont = FontManager.defaultFont;

        public int itemsSpacing = 0;

        private Microsoft.Xna.Framework.Vector2 textv2;

        public Dropdown()
        {
            LeftClick += Dropdown_LeftClick;
        }

        public Dropdown(List<DropdownItem> items)
        {
            this.items = items;
            LeftClick += Dropdown_LeftClick;
        }

        public Dropdown(DropdownItem[] items)
        {
            this.items = new List<DropdownItem>(items);
            LeftClick += Dropdown_LeftClick;
        }

        private void Dropdown_LeftClick(object sender, EventArgs e)
        {
            debugDrawElementRectangle = true;
            Console.WriteLine("clicked");
            isOpen = !isOpen;
        }

        internal override void DrawUI(Drawer spritesBatch)
        {
            var elementRectangle = ElementRectangle;
            elementRectangle.Height = selectedItem.height * (int)transform.scale.Y;
            elementRectangle.Y = (int)transform.position.Y;

            Microsoft.Xna.Framework.Vector2 center = new Microsoft.Xna.Framework.Vector2(elementRectangle.X + elementRectangle.Width / 2 - textv2.X / 2,
                elementRectangle.Y + elementRectangle.Height / 2 - textv2.Y / 2);

            DrawSelectionBox(spritesBatch, selectedItem, elementRectangle, center);

            elementRectangle.Height += selectedItem.height;

            if (isOpen && items != null && isSelected)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    elementRectangle.Y -= items[i].height;
                    center.X = elementRectangle.X + elementRectangle.Width / 2 - listv2[i].X / 2;
                    center.Y -= items[i].height;
                    DrawSelectionBox(spritesBatch, items[i], elementRectangle, center);
                }
            }
        }

        private void DrawSelectionBox(Drawer spritesBatch, DropdownItem dropdownItem, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.Vector2 position)
        {
            spritesBatch.shapes.DrawFilledRectangle(rect, dropdownItem.backgroundColor);
            spritesBatch.DrawString(textFont.font, dropdownItem.text, position, dropdownItem.textColor.color);
        }

        internal override Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            var rect = new Microsoft.Xna.Framework.Rectangle((int)transform.position.X,
            (int)transform.position.Y,
            selectedItem.width * (int)transform.scale.X, selectedItem.height * (int)transform.scale.Y);

            if (isOpen && items != null && isSelected)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    rect.Height += items[i].height * (int)transform.scale.Y;
                    rect.Y -= items[i].height;
                }
            }
            //Console.WriteLine(rect);
            return rect;

        }

    }

    public class DropdownItem
    {
        public bool isSelected { get; internal set; }

        public string text = "";
        public object obj;

        public Color backgroundColor = Color.White;
        public Color hoveringColor = Color.Gray;

        public Color textColor = Color.Black;

        public int width = 200;
        public int height = 50;

        public event EventHandler OnSelectionChange;

        public DropdownItem()
        {

        }

        public DropdownItem(string text)
        {
            this.text = text;
        }

        public DropdownItem(string text, Color backgroundColor, Color hoveringColor, Color textColor, int width, int height)
        {
            this.text = text;

            this.backgroundColor = backgroundColor;
            this.hoveringColor = hoveringColor;

            this.textColor = textColor;

            this.width = width;
            this.height = height;
        }

    }

}