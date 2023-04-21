using MonoGame.Extended;

namespace boreal.engine
{
    public class Circle
    {
        public Point2 center;
        public float size;

        public Circle(Point2 center, float size)
        {
            this.center = center;
            this.size = size;
        }

        internal CircleF ToCircleF()
        {
            return new CircleF(new MonoGame.Extended.Point2(center.X, center.Y), size);
        }

    }
}
