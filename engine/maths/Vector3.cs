using System;

namespace boreal.engine
{

    public class Vector3
    {
        internal Microsoft.Xna.Framework.Vector3 xnaV3;

        public float X
        {
            get { return xnaV3.X; }
            set { xnaV3.X = value; }
        }

        public float Y
        {
            get { return xnaV3.Y; }
            set { xnaV3.Y = value; }
        }
        public float Z
        {
            get { return xnaV3.Z; }
            set { xnaV3.Z = value; }
        }

        public Vector3(float v)
        {
            X = v;
            Y = v;
            Z = v;
        }

        public Vector3(Vector2 vector2) 
        {
            X = vector2.X;
            Y = vector2.Y;
            Z = 0;
        }

        public Vector3(Vector2 vector2, float z)
        {
            X = vector2.X;
            Y = vector2.Y;
            Z = z;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static float Distance(Vector3 value1, Vector3 value2)
        {
            float x = MathF.Abs((MathF.Max(value1.X, value2.X) - MathF.Min(value1.X, value2.X)));
            float y = MathF.Abs((MathF.Max(value1.Y, value2.Y) - MathF.Min(value1.Y, value2.Y)));
            float z = MathF.Abs((MathF.Max(value1.Z, value2.Z) - MathF.Min(value1.Z, value2.Z)));

            return MathF.Sqrt(x * x + y * y + z * z);
        }


        #region operators

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            a.X += b.X;
            a.Y += b.Y;
            a.Z += b.Z;
            return a;
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            a.X += b;
            a.Y += b;
            a.Z += b;
            return a;
        }
        public static Vector3 operator -(Vector3 a, float b)
        {
            return new Vector3(a.X - b, a.Y - b, a.Z - b);
        }
        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }
        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3 operator +(float a, Vector3 b)
        {
            return new Vector3(a + b.X, a + b.Y, a + b.Z);
        }
        public static Vector3 operator -(float a, Vector3 b)
        {
            return new Vector3(a - b.X, a - b.Y, a - b.Z);
        }
        public static Vector3 operator *(float a, Vector3 b)
        {
            return new Vector3(a * b.X, a * b.Y, a * b.Z);
        }
        public static Vector3 operator /(float a, Vector3 b)
        {
            return new Vector3(a / b.X, a / b.Y, a / b.Z);
        }

        #endregion operators

        public override string ToString()
        {
            return X + ";" + Y + ";" + Z;
        }
    }

}
