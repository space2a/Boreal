using System;

namespace boreal.engine
{
    public class Point
    {

        public int X, Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(int value)
        {
            X = value;
            Y = value;
        }

        internal Point(Microsoft.Xna.Framework.Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        internal Microsoft.Xna.Framework.Point ToXna()
        {
            return new Microsoft.Xna.Framework.Point(X, Y);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        #region operators

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point p1, Point p2)
        {
            return new Point(p1.X * p2.X, p1.Y * p2.Y);
        }

        public static Point operator /(Point p1, Point p2)
        {
            return new Point(p1.X / p2.X, p1.Y / p2.Y);
        }

        public static Point operator +(Vector2 v1, Point p2)
        {
            return new Point((int)v1.X + p2.X, (int)v1.Y + p2.Y);
        }

        public static Point operator -(Vector2 v1, Point p2)
        {
            return new Point((int)v1.X - p2.X, (int)v1.Y - p2.Y);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            if (p1 is null || p2 is null) return ReferenceEquals(p1, p2);
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            if (p1 is null || p2 is null) return !ReferenceEquals(p1, p2);
            return p1.X != p2.X && p1.Y != p2.Y;
        }

        #endregion operators

        public static float Distance(Point value1, Point value2)
        {
            float x = MathF.Abs((MathF.Max(value1.X, value2.X) - MathF.Min(value1.X, value2.X)));
            float y = MathF.Abs((MathF.Max(value1.Y, value2.Y) - MathF.Min(value1.Y, value2.Y)));

            return MathF.Sqrt(x * x + y * y);
        }

        public override string ToString()
        {
            return X + ";" + Y;
        }

        public override bool Equals(object p)
        {
            return (p as Point) == this;
        }
    }
}
