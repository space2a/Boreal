using System;

using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;

namespace boreal.engine
{
    public class UIComponent : Component
    {
        public event EventHandler LeftClick, LeftClickHold, LeftClickReleased;
        public event EventHandler RightClick;
        public event EventHandler MiddleClick;
        public event EventHandler AnyClick;

        public bool isHovering { get; private set; }
        public event EventHandler isHoveringChanged;

        public bool calculHovering = true;

        private static UIComponent selectedUIComponent;

        public bool isSelected
        {
            get { return (selectedUIComponent == this); }
            set { if (value) selectedUIComponent = this; else selectedUIComponent = null; }
        }

        protected bool scissorsElementRectangle = false;
        protected bool persistentScissorsElementRectangle = false;

        internal static Microsoft.Xna.Framework.Rectangle worldMouseRectangle;
        internal static Microsoft.Xna.Framework.Rectangle windowMouseRectangle;
        internal static UIComponent hoveringUIComponent = null;

        private Microsoft.Xna.Framework.Input.MouseState currentMouseState;
        private Microsoft.Xna.Framework.Input.MouseState previousMouseState;

        public bool debugDrawElementRectangle = true;

        internal Microsoft.Xna.Framework.Rectangle ElementRectangle
        {
            get
            {
                if(transform == null) return new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0);
                else return GetElementRectangle();
            }
        }


        public boreal.engine.Rectangle elementRectangle
        {
            get
            {
                if (transform == null) return new engine.Rectangle(0, 0, 0, 0);
                else return new engine.Rectangle(GetElementRectangle());
            }
        }


        protected override void Update(GameTime gameTime)
        {
            MouseClicksInformation mci = Inputs.MouseState.GetClicks();
            if (calculHovering)
            {
                bool isIntersecting = false;

                if (!gameObject.isInCanvas)
                    isIntersecting = worldMouseRectangle.Intersects(ElementRectangle);
                else
                {
                    isIntersecting = windowMouseRectangle.Intersects(ElementRectangle);
                }

                if (isIntersecting && hoveringUIComponent == null || isIntersecting && hoveringUIComponent == this)
                {
                    //NEED TO CHECK FOR LAYERING. if(hoveringUIComponent.layer < this.layer) hoveringUIComponent = this;
                    if (isIntersecting != isHovering) 
                        isHoveringChanged?.Invoke(this, new EventArgs());

                    isHovering = isIntersecting;
                    hoveringUIComponent = this;
                }
                else if (!isIntersecting && isIntersecting != isHovering) 
                { hoveringUIComponent = null; isHovering = false; } //Possible fix pour le hovering à travers multiples ui comps
            }
            else isHovering = false;

            if (isHovering)
            {
                previousMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();

                if (mci.hasAnyClick)
                {
                    isSelected = true;
                    AnyClick?.Invoke(this, new EventArgs());
                }

                if (LeftClick != null && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                { 
                    LeftClick.Invoke(this, new EventArgs()); 
                }
                else if (LeftClickHold != null && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    LeftClickHold.Invoke(this, new EventArgs());
                }
                else if (LeftClickReleased != null && currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Released)
                {
                    LeftClickReleased.Invoke(this, new EventArgs());
                }
                else if (RightClick != null && currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                { 
                    RightClick.Invoke(this, new EventArgs()); 
                }
                else if (MiddleClick != null && currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed)
                { 
                    MiddleClick.Invoke(this, new EventArgs());
                }
            }
            else
            {
                if (mci.hasAnyClick && isSelected)
                    isSelected = false;
            }

            base.Update(gameTime);
        }


        internal override void PreDrawUI(Sprites spritesBatch)
        {
            var elementRectangle = ElementRectangle;

            if (debugDrawElementRectangle)
            {
                if (isSelected)
                    spritesBatch.shapes.DrawCircleOutline(new CircleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), 20), engine.Color.Cyan, 10);
                else
                    spritesBatch.shapes.DrawCircleOutline(new CircleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), 20), engine.Color.Red, 10);


                if (isHovering)
                    spritesBatch.shapes.DrawCircleOutline(new CircleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), 10), engine.Color.Green, 10);
                else
                    spritesBatch.shapes.DrawCircleOutline(new CircleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), 10), engine.Color.Red, 10);
            }

            if (debugDrawElementRectangle)
                spritesBatch.shapes.DrawRectangleOutline(elementRectangle, Color.Magenta, 1);

            if (scissorsElementRectangle)
            {
                spritesBatch.SetScissorRectangle(elementRectangle);
                
                DrawUI(spritesBatch);

                if (!persistentScissorsElementRectangle)
                {
                    spritesBatch.ResetScissorRectangle();
                }
            }
            else
                DrawUI(spritesBatch);
        }

        internal override void DrawUI(Sprites spritesBatch)
        {

        }

        internal override void EndDrawUI(Sprites spritesBatch)
        {
            if(persistentScissorsElementRectangle)
                spritesBatch.ResetScissorRectangle();
        }


        internal virtual Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(0, 0, 10, 10);
        }
    }
}
