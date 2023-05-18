using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public class Vector2
    {
        internal Microsoft.Xna.Framework.Vector2 xnaV2 = new Microsoft.Xna.Framework.Vector2();

        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 One = new Vector2(1, 1);

        public static readonly Vector2 Up = new Vector2(0, 1);
        public static readonly Vector2 Down = new Vector2(0, -1);
        public static readonly Vector2 Right = new Vector2(-1, 0);
        public static readonly Vector2 Left = new Vector2(1, 0);

        public float X
        {
            get { return xnaV2.X; }
            set { xnaV2.X = value; }
        }

        public float Y
        {
            get { return xnaV2.Y; }
            set { xnaV2.Y = value; }
        }

        public static Vector2 GetPosition(Position position, Rectangle boundingRectangle = null)
        {
            Rectangle brectangle = null;
            if (boundingRectangle == null) { brectangle = Launcher.core.windowProfile.boundingRectangle; } else brectangle = boundingRectangle;

            switch (position)
            {
                case Position.TopLeft:
                    return new Vector2(brectangle.Width * -1 /2, brectangle.Height /2);
                case Position.TopCenter:
                    return new Vector2(0, brectangle.Height /2);
                case Position.TopRight:
                    return new Vector2(brectangle.Width / 2, brectangle.Height / 2);

                case Position.CenterLeft:
                    return new Vector2(brectangle.Width * -1 /2, 0);
                case Position.Center:
                    return new Vector2(0, 0);
                case Position.CenterRight:
                    return new Vector2(brectangle.Width/2, 0);

                case Position.BottomLeft:
                    return new Vector2(brectangle.Width * -1 /2, brectangle.Height * -1 /2);
                case Position.BottomCenter:
                    return new Vector2(0, brectangle.Height * -1 /2);
                case Position.BottomRight:
                    return new Vector2(brectangle.Width /2, brectangle.Height * -1 /2);

                default:
                    return Vector2.Zero;
            }
        }

        public enum Position
        {
            TopLeft, TopCenter, TopRight,
            CenterLeft, Center, CenterRight,
            BottomLeft, BottomCenter, BottomRight
        }


        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        public Vector2(Vector2 vector2)
        {
            X = vector2.X;
            Y = vector2.Y;
        }

        internal Vector2(Microsoft.Xna.Framework.Vector2 vector2)
        {
            X = vector2.X;
            Y = vector2.Y;
        }

        #region operators

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X / b.X, a.Y / b.Y);
        }

        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2(a.X + b, a.Y + b);
        }
        public static Vector2 operator -(Vector2 a, float b)
        {
            return new Vector2(a.X - b, a.Y - b);
        }
        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }
        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

        public static Vector2 operator +(float a, Vector2 b)
        {
            return new Vector2(a + b.X, a + b.Y);
        }
        public static Vector2 operator -(float a, Vector2 b)
        {
            return new Vector2(a - b.X, a - b.Y);
        }
        public static Vector2 operator *(float a, Vector2 b)
        {
            return new Vector2(a * b.X, a * b.Y);
        }
        public static Vector2 operator /(float a, Vector2 b)
        {
            return new Vector2(a / b.X, a / b.Y);
        }

        public static Vector2 operator +(Vector2 a, Point b)
        {
            return new Vector2(a.X + b.X, a.X + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Point b)
        {
            return new Vector2(a.X - b.X, a.X - b.Y);
        }

        public static Vector2 operator *(Vector2 a, Point b)
        {
            return new Vector2(a.X * b.X, a.X * b.Y);
        }

        public static Vector2 operator /(Vector2 a, Point b)
        {
            return new Vector2(a.X / b.X, a.X / b.Y);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            if (a is null || b is null) return ReferenceEquals(a, b);
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            if (a is null || b is null) return !ReferenceEquals(a, b);
            return a.X != b.X && a.Y != b.Y;
        }


        #endregion operators

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        public bool isNaN()
        {
            if (!float.IsNaN(X))
            {
                return float.IsNaN(Y);
            }
            return true;
        }

        public static bool isNaN(Vector2 vector2)
        {
            if (!float.IsNaN(vector2.X))
            {
                return float.IsNaN(vector2.Y);
            }
            return true;
        }

        public static Vector2 Subtract(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static Vector2 Normalize(Vector2 value, bool canBeNaN = false)
        {
            value.Normalize();
            return value;
        }

        public void Normalize(bool canBeNaN = false)
        {
            float num = 1f / MathF.Sqrt(X * X + Y * Y);
            X *= num;
            Y *= num;
            if (!canBeNaN & isNaN()) { X = 0; Y = 0; }
        }

        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float x = MathF.Abs((MathF.Max(value1.X, value2.X) - MathF.Min(value1.X, value2.X)));
            float y = MathF.Abs((MathF.Max(value1.Y, value2.Y) - MathF.Min(value1.Y, value2.Y)));

            return MathF.Sqrt(x * x + y * y);
        }

        public static float Cross(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.Y
                   - value1.Y * value2.X;
        }

        public static float Dot(Vector2 value1, Vector2 value2)
        {
            return (value1.X * value2.X)
                 + (value1.Y * value2.Y);
        }

        public override string ToString()
        {
            return X + ";" + Y;
        }

        internal static Vector2 ParseVector2(Microsoft.Xna.Framework.Vector2 vector2) { return new Vector2(vector2.X, vector2.Y); }

        internal Microsoft.Xna.Framework.Vector3 ToXNAVector3() { return new Microsoft.Xna.Framework.Vector3(X, Y, 0); }
    }
}