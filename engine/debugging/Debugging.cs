using MonoGame.Extended;
using MonoGame.Extended.Serialization;

using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public static class Debugging
    {
        public static List<DrawingObject> drawingObjects = new List<DrawingObject>();

        private static Sprites spriteBatch
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

        internal static void Draw(Sprites spriteBatch)
        {
            CallDraws(drawingObjects.FindAll(x => x.drawLevel == DrawingObject.DrawLevel.Base), spriteBatch);
        }


        internal static void DrawUI(Sprites spriteBatch)
        {
            CallDraws(drawingObjects.FindAll(x => x.drawLevel == DrawingObject.DrawLevel.UI), spriteBatch);
        }

        private static void CallDraws(List<DrawingObject> drawingObjects, Sprites spriteBatch) 
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
        internal static Sprites spriteBatch
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
                spriteBatch.shapes.DrawFilledRectangle(rectangle, color);
            else
                spriteBatch.shapes.DrawRectangleOutline(rectangle, color, thickness);
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
            spriteBatch.shapes.DrawCircleOutline(circle.ToCircleF(), color, thickness);
        }
    }


}
