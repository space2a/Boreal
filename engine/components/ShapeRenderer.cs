using boreal.engine.graphics;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Runtime.InteropServices;

namespace boreal.engine
{
    public class ShapeRenderer : Component
    {
        public Shape shape;

        public ShapeRenderer()
        {

        }

        public ShapeRenderer(Shape shape)
        {
            this.shape = shape;
            shape.transform = base.transform;
        }

        internal override void Draw(Sprites spritesBatch)
        {
            shape?.Draw(spritesBatch, transform);
        }

        internal override void DestroyComponent()
        {
            shape?.Destroy();
        }

    }

    public class Shape
    {
        internal Transform transform;
        internal virtual void Draw(Sprites spritesBatch, Transform transform) { }
        internal virtual void Destroy() { }
    }

    public class ShapeRectangle : Shape
    {
        public Color color;
        public Gradient gradient;

        public ShapeRectangle(Color color)
        {
            this.color = color;
        }

        public ShapeRectangle(Gradient gradient)
        {
            this.gradient = gradient;
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if (color != null)
            {
                spritesBatch.shapes.DrawFilledRectangle(
                new Microsoft.Xna.Framework.Rectangle((int)transform.position.X, (int)transform.position.Y,
                (int)transform.scale.X, (int)transform.scale.Y), color);
            }
            else if (gradient != null)
            {
                spritesBatch.shapes.DrawFilledRectangleGradient(
                new Microsoft.Xna.Framework.Rectangle((int)transform.position.X, (int)transform.position.Y,
                (int)transform.scale.X, (int)transform.scale.Y), gradient);
            }
        }
    }

    public class ShapeCustomRectangle : Shape
    {
        public Color color;
        public Gradient gradient;

        public Rectangle rectangle;

        public ShapeCustomRectangle(Rectangle rectangle, Color color)
        {
            this.color = color;
        }

        public ShapeCustomRectangle(Rectangle rectangle, Gradient gradient)
        {
            this.gradient = gradient;
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if (color != null)
            {
                spritesBatch.shapes.DrawFilledRectangle(rectangle.rect, color);
            }
            else if (gradient != null)
            {
                spritesBatch.shapes.DrawFilledRectangleGradient(
                rectangle.rect, gradient);
            }
        }
    }

    public class ShapeLine : Shape
    {
        public Color color;
        public Gradient gradient;

        public Point2 a, b;
        public float thickness = 0;

        public ShapeLine(Point2 a, Point2 b, float thickness, Color color)
        {
            this.color = color;
            this.a = a;
            this.b = b;
            this.thickness = thickness;
        }

        public ShapeLine(Point2 a, Point2 b, float thickness, Gradient gradient)
        {
            this.gradient = gradient;
            this.a = a;
            this.b = b;
            this.thickness = thickness;
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if (color != null)
            {
                spritesBatch.shapes.DrawLine(a.X, a.Y, b.X, b.Y, thickness, color);
            }
            else if (gradient != null)
            {
                spritesBatch.shapes.DrawLineGradient(a.X, a.Y, b.X, b.Y, thickness, gradient);
            }
        }
    }

    public class ShapeCircle : Shape
    {
        public Color color;

        public float radius = 0;
        public float thickness = 0;

        public int points = 0;

        public bool filled = false;

        public ShapeCircle(float radius, int points, float thickness, bool filled, Color color)
        {
            this.color = color;
            this.radius = radius;
            this.points = points;
            this.thickness = thickness;
            this.filled = filled;
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if (filled)
                spritesBatch.shapes.DrawCircleFilled(transform.position.X, transform.position.Y, radius, points, color);
            else
                spritesBatch.shapes.DrawCircle(transform.position.X, transform.position.Y, radius, points, thickness, color);
        }
    }

    public class ShapePolygon : Shape
    {
        public Color[] color;

        public Vector2[] vertices;
        public Vector3[] vertices3D;

        private VertexPositionColor[] vpc3D;

        public int[] indices;

        public float thickness = 0;

        public ShapePolygon(Vector2[] vertices, float thickness, Color[] color)
        {
            this.color = color;

            this.vertices = vertices;
            this.thickness = thickness;
        }

        public ShapePolygon(Vector2[] vertices, bool fill, Color[] color)
        {
            this.color = color;

            this.vertices = vertices;
            if (fill)
            {
                PolygonHelper.Triangulate(vertices, out indices, out string err);
                Console.WriteLine(err);
            }
        }

        public ShapePolygon(Vector3[] vertices, int[] triangles, Color[] color)
        {
            this.color = color;

            this.vertices3D = vertices;
            this.indices = triangles;

            vpc3D = new VertexPositionColor[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                vpc3D[i] = new VertexPositionColor(
                    new Microsoft.Xna.Framework.Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z), color[i].color); //TRANSFORM OPERATION POSITION + SCALE
            }
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if (indices == null)
                spritesBatch.shapes.DrawPolygon(vertices, thickness, color, transform.position);
            else
            {
                spritesBatch.shapes.DrawPolygonFilled(vpc3D, indices, color);
            }
        }

        internal override void Destroy()
        {
            vertices = null;
            vertices3D = null;
            indices = null;
            color = null;
        }
    }

}
