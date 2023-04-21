namespace boreal.engine
{
    public class Gradient
    {

        public Color[] colors { get; private set; }

        public Gradient(Color first, Color second, Color third, Color fourth)
        {
            colors = new Color[4] {first, second, third, fourth };
        }

        public Gradient(Color first, Color second)
        {
            colors = new Color[2] { first, second };
        }

        public Color this[int index]
        {
            get { return colors[index]; }
        }
    }
}
