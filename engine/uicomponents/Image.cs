namespace boreal.engine
{
    public class Image : UIComponent
    {
        public Sprite sprite;

        public Image()
        {

        }

        public Image(Sprite sprite)
        {
            this.sprite = sprite;
        }

        internal override void DrawUI(Sprites spritesBatch)
        {
            sprite?.Draw(spritesBatch, transform);
        }

        internal override void DestroyComponent()
        {
            sprite?.texture2D.Dispose();
        }

    }
}
