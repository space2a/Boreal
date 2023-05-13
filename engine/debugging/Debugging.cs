using System.Collections.Generic;

namespace boreal.engine
{
    public static class Debugging
    {
        public static List<DrawingObject> drawingObjects = new List<DrawingObject>();
        public static int drawOrder = -999;
        private static Drawer spriteBatch
        {
            get
            {
                return Launcher.core.spritesBatch;
            }
        }

        public static void AddDrawingObject(DrawingObject drawingObject)
        {
            drawingObjects.Add(drawingObject);
        }

        public static void RemoveDrawingObject(DrawingObject drawingObject)
        {
            drawingObjects.Remove(drawingObject);
        }

        internal static void Draw(Drawer spriteBatch)
        {
            CallDraws(drawingObjects.FindAll(x => x.drawLevel == DrawingObject.DrawLevel.Base), spriteBatch);
        }


        internal static void DrawUI(Drawer spriteBatch)
        {
            CallDraws(drawingObjects.FindAll(x => x.drawLevel == DrawingObject.DrawLevel.UI), spriteBatch);
        }

        private static void CallDraws(List<DrawingObject> drawingObjects, Drawer spriteBatch) 
        {
            for (int i = 0; i < drawingObjects.Count; i++)
            {
                drawingObjects[i].Draw();
                if (!drawingObjects[i].persistent)
                    drawingObjects.Remove(drawingObjects[i]);
            }
        }

    }

    public abstract class DrawingObject
    {
        public bool persistent = false;

        public Color color;

        public DrawLevel drawLevel;
        internal static Drawer spriteBatch
        {
            get
            {
                return Launcher.core.spritesBatch;
            }
        }

        public enum DrawLevel
        {
            Base,
            UI
        }

        public DrawingObject(Color color, DrawLevel drawLevel, bool persistent = false)
        {
            this.color = color;
            this.drawLevel = drawLevel;
            this.persistent = persistent;
        }

        internal virtual void Draw() { }
    }

    public class DrawingObjectRectangle : DrawingObject
    {
        private Microsoft.Xna.Framework.Rectangle rectangle;
        private bool isFiled = false;
        private float thickness;

        public DrawingObjectRectangle(engine.Rectangle rectangle, bool filled, float thickness, Color color, DrawLevel drawLevel, bool persistent = false) : base(color, drawLevel, persistent)
        { this.rectangle = rectangle.rect; this.isFiled = filled; this.thickness = thickness; }

        internal override void Draw()
        {
            if (isFiled)
                spriteBatch.shapes.DrawFilledRectangle(rectangle, color, Debugging.drawOrder);
            else
                spriteBatch.shapes.DrawRectangleOutline(rectangle, color, thickness, Debugging.drawOrder);
        }
    }

    public class DrawingObjectCircle : DrawingObject
    {
        public Circle circle;

        private int thickness;

        public DrawingObjectCircle(engine.Circle circle, Color color, int thickness, DrawLevel drawLevel, bool persistent = false) : base(color, drawLevel, persistent)
        { this.circle = circle; this.thickness = thickness; }

        internal override void Draw()
        {
            spriteBatch.shapes.DrawCircleOutline(circle.ToCircleF(), color, thickness, Debugging.drawOrder);
        }
    }

    public class DrawingObjectText : DrawingObject
    {
        public string text;
        public Vector2 position;
        public float scale;

        public DrawingObjectText(string text, Vector2 position, float scale, Color color, DrawLevel drawLevel, bool persistent = false) : base(color, drawLevel, persistent)
        {
            this.text = text;
            this.position = position;
            this.scale = scale;
        }

        internal override void Draw()
        {
            spriteBatch.DrawString(FontManager.defaultFont.font, text, position.xnaV2, color.color, orderBy: Debugging.drawOrder, scale: scale);
        }
    }


}
