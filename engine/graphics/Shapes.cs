using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using System;
using System.Collections.Generic;

namespace boreal.engine
{
    internal class Shapes : IDisposable
    {
        private bool isDisposed;
        internal BasicEffect basicEffect;

        private List<VertexPositionColor> vertices = new List<VertexPositionColor>();
        private List<int> indices = new List<int>();

        public bool isStarted { get; private set; }

        private Core core;
        public SpriteBatch spriteBatch;

        public BasicEffect emptyEffect;

        internal Shapes(Core core)
        {
            this.core = core;

            basicEffect = new BasicEffect(core.GraphicsDevice)
            {
                TextureEnabled = false,
                FogEnabled = false,
                VertexColorEnabled = true,
                Projection = Matrix.Identity
            };

            emptyEffect = new BasicEffect(core.GraphicsDevice)
            {
                Projection = Matrix.Identity,
                View = Matrix.Identity,
                World = Matrix.Identity,
                VertexColorEnabled = true,
                LightingEnabled = false,
                TextureEnabled = false,
                FogEnabled = false,
                Alpha = 1
            };

            isStarted = false;
        }

        public void Dispose()
        {
            if (isDisposed) return;
            basicEffect?.Dispose();
            emptyEffect?.Dispose();
            isDisposed = true;
        }

        public void Begin(Camera camera)
        {
            if (isStarted) throw new Exception("already started");

            basicEffect.View = camera.view;

            basicEffect.Projection = camera.proj;

            isStarted = true;
        }

        public void End()
        {
            //Flush();

            isStarted = false;
        }

        public bool isDrawingInCanvas = false;

        public void Flush()
        {
            BasicEffect be = basicEffect;

            if (isDrawingInCanvas) be = emptyEffect;

            foreach (EffectPass pass in be.CurrentTechnique.Passes)
            {
                pass.Apply();
                core.GraphicsDevice.DrawUserIndexedPrimitives
                    (PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count, indices.ToArray(), 0, indices.Count / 3);
            }

            vertices.Clear();
            indices.Clear();
        }

        public void EnsureStarted()
        {
            //if (!isStarted) throw new Exception("not started");
        }

        public void EnsureSpace(int vertexCount, int indexCount)
        {
            //if (vertices.Count > vertices.Length) throw new Exception("max shape vertex count is " + vertices.Length);
            //if (indexCount > indices.Length) throw new Exception("max shape index count is " + indices.Length);
            //
            //if (this.vertices.Count + vertices.Count > vertices.Length
            //    || this.indexCount + indexCount > vertices.Length) { Console.WriteLine("EnsureSpace Flush"); Flush(); }
        }

        public void SetMatrixToDefault()
        {
            emptyEffect.Projection =
                Matrix.CreateOrthographicOffCenter(0, core.GraphicsDevice.Viewport.Width,
                0, core.GraphicsDevice.Viewport.Height, 0, 10);
        }

        public void DrawFilledRectangle(Microsoft.Xna.Framework.Rectangle rectangle, Color color, int layer = 0)
        {
            core.spritesBatch.CreateDrawAction(() => DrawFilledRectangleGradient(rectangle, new Gradient(color, color, color, color)), layer);
        }

        public void DrawFilledRectangleGradient(Microsoft.Xna.Framework.Rectangle rectangle, Gradient gradient)
        {
            EnsureStarted();
            //const int shapevertices.Count = 4;
            //const int shapeIndexCount = 6;
            //EnsureSpace(shapevertices.Count, shapeIndexCount);

            float left = rectangle.X;
            float right = rectangle.X + rectangle.Width;

            float top = rectangle.Y + rectangle.Height;
            float bottom = rectangle.Y;

            var a = new Microsoft.Xna.Framework.Vector2(left, top);
            var b = new Microsoft.Xna.Framework.Vector2(right, top);
            var c = new Microsoft.Xna.Framework.Vector2(right, bottom);
            var d = new Microsoft.Xna.Framework.Vector2(left, bottom);


            indices.Add(0 + vertices.Count);
            indices.Add(1 + vertices.Count);
            indices.Add(2 + vertices.Count);
            indices.Add(0 + vertices.Count);
            indices.Add(2 + vertices.Count);
            indices.Add(3 + vertices.Count);

            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(a, 0f), gradient[0].color));
            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(b, 0f), gradient[1].color));
            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(c, 0f), gradient[2].color));
            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(d, 0f), gradient[3].color));

            basicEffect.Alpha = (float)gradient[0].color.A / 255;
            Flush();
        }

        public void DrawRectangleOutline(Microsoft.Xna.Framework.Rectangle rectangle, Color color, float thickness, int layer = 0)
        {
            core.spritesBatch.CreateDrawAction(() => spriteBatch.DrawRectangle(rectangle, color.color, thickness, 0), layer);
        }

        public void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color, bool flush = true)
        {
            DrawLineGradient(ax, ay, bx, by, thickness, new Gradient(color, color, color, color), flush);
        }

        public void DrawLineGradient(float ax, float ay, float bx, float by, float thickness, Gradient gradient, bool flush = true)
        {
            EnsureStarted();

            //EnsureSpace(shapevertices.Count, shapeIndexCount);

            if (thickness < 0) thickness = 0;

            float halfThickness = thickness / 2;

            float e1x = bx - ax;
            float e1y = by - ay;

            MathU.Normalize(ref e1x, ref e1y);

            e1x *= halfThickness;
            e1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n1x = -e1y;
            float n1y = e1x;

            float n2x = -n1x;
            float n2y = -n1y;

            float q1x = ax + n1x + e2x;
            float q1y = ay + n1y + e2y;

            float q2x = bx + n1x + e1x;
            float q2y = by + n1y + e1y;

            float q3x = bx + n2x + e1x;
            float q3y = by + n2y + e1y;

            float q4x = ax + n2x + e2x;
            float q4y = ay + n2y + e2y;

            indices.Add(0 + vertices.Count);
            indices.Add(1 + vertices.Count);
            indices.Add(2 + vertices.Count);
            indices.Add(0 + vertices.Count);
            indices.Add(2 + vertices.Count);
            indices.Add(3 + vertices.Count);

            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q1x, q1y, 0), gradient[0].color));
            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q2x, q2y, 0), gradient[1].color));
            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q3x, q3y, 0), gradient[2].color));
            vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q4x, q4y, 0), gradient[3].color));

            if (flush)
                Flush();
        }

        internal void DrawCircleOutline(CircleF circle, Color color, int thickness, int layer = 0)
        {
            core.spritesBatch.CreateDrawAction(() => spriteBatch.DrawCircle(circle, 10, color.color, thickness, 0), layer);
        }

        public void DrawCircle(float x, float y, float radius, int points, float thickness, Color color)
        {
            if (points < 3) points = 3;

            float rotation = MathHelper.TwoPi / (float)points;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            float bx = 0;
            float by = 0;

            for (int i = 0; i < points; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                this.DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

                ax = bx;
                ay = by;
            }

            Flush();
        }

        public void DrawCircleFilled(float x, float y, float radius, int points, Color color)
        {
            if (points < 3) points = 3;

            EnsureStarted();

            int shapeVertexCount = points;
            int shapeTriangleCount = shapeVertexCount - 2;
            int shapeIndexCount = shapeTriangleCount * 3;
            //EnsureSpace(shapevertices.Count, shapeIndexCount);

            int index = 1;

            for (int i = 0; i < shapeTriangleCount; i++)
            {
                indices.Add(0 + this.vertices.Count);
                indices.Add(index + this.vertices.Count);
                indices.Add(index + 1 + this.vertices.Count);

                index++;
            }

            float rotation = MathHelper.TwoPi / (float)points;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < shapeVertexCount; i++)
            {
                float x1 = ax;
                float y1 = ay;

                vertices.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(x1 + x, y1 + y, 0f), color.color));

                ax = cos * x1 - sin * y1;
                ay = sin * x1 + cos * y1;
            }
            Flush();
        }

        public void DrawPolygon(Vector2[] vertices, float thickness, Color[] color, Vector2 origin = null)
        {
            if (origin == null) origin = Vector2.Zero;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = origin + vertices[i];
                Vector2 b = origin + vertices[(i + 1) % vertices.Length];

                this.DrawLine(a.X, a.Y, b.X, b.Y, thickness, color[i], false);
            }

            Flush();
        }

        public void DrawPolygonFilled(VertexPositionColor[] vertices, int[] triangleIndices, Color[] color)
        {
            this.vertices.AddRange(vertices);
            this.indices.AddRange(triangleIndices);

            Flush();
        }

    }
}
