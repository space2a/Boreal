using System;

namespace boreal.engine
{
    public static class MathU
    {

        public static float Clamp(float value, float min, float max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("max > min");
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("max > min");
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static float CalculPourcent(float number, float pourcent)
        {
            return (pourcent * number / 100);
        }

        public static int CalculPourcent(int number, int pourcent)
        {
            return (pourcent * number / 100);
        }

        public static float ToRadians(float degrees)
        {
            return (float)((double)degrees * (Math.PI / 180.0));
        }

        public static void Normalize(ref float x, ref float y)
        {
            float len = 1f /MathF.Sqrt(x * x + y * y);
            x *= len;
            y *= len;
        }
    }
}
