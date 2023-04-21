namespace boreal.engine
{
    public class Container : UIComponent
    {

        protected override void Start()
        {
            scissorsElementRectangle = true;
            persistentScissorsElementRectangle = true;
        }

        internal override Microsoft.Xna.Framework.Rectangle GetElementRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(
                (int)transform.position.X, (int)transform.position.Y,
                (int)transform.scale.X, (int)transform.scale.Y);
        }

    }
}
