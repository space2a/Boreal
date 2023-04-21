using System.Collections.Generic;

namespace boreal.engine
{
    public class MultiTextureRenderer : Component
    {
        public List<TextureObject> textureObjects = new List<TextureObject>();

        public Vector2 loc = new Vector2(0);
        public float angle = -38;
        public float angleY = 0;

        internal override void Draw(Sprites spritesBatch)
        {
            //spritesBatch.sprites.End();

            //spritesBatch.Begin(spritesBatch.camera, false, false, SpriteSortMode.Deferred);
            for (int i = 0; i < textureObjects.Count; i++)
            {
                var tObject = textureObjects[i];

                tObject.BeforeDraw();

                if (tObject.texture2D != null)
                {
                    spritesBatch.Draw(tObject.texture2D.texture2D, null, tObject.origin.xnaV2,
                    (transform.position + tObject.drawPosition).xnaV2,
                    tObject.rotation + transform.rotation, tObject.drawSize.xnaV2 * transform.scale.xnaV2, tObject.drawColor);
                }
                else
                {
                    tObject.sprite.Draw(spritesBatch, (transform.position + tObject.drawPosition), tObject.drawSize * transform.scale, tObject.origin, transform, tObject.drawColor);
                }

                tObject.AfterDraw();
            }

            //spritesBatch.End();
            //spritesBatch.Begin(spritesBatch.camera, false);
        }

    }

    public class TextureObject
    {
        public Texture2D texture2D = null;
        public Sprite sprite = null;

        public Vector2 drawPosition = new Vector2(0);
        public Vector2 origin = new Vector2(0);
        public Vector2 drawSize = new Vector2(1);

        public float rotation = 0;

        public Color drawColor = Color.White;

        public TextureObject()
        {

        }

        public TextureObject(Texture2D texture2D, Vector2 drawPosition, Color drawColor = null, Vector2 drawSize = null)
        {
            this.texture2D = texture2D;
            this.drawPosition = drawPosition;

            if (drawColor == null)
                drawColor = Color.White;

            if (drawSize == null)
                drawSize = new Vector2(1);

            origin = new Vector2((texture2D.width * drawSize.X) /2, (texture2D.height * drawSize.Y) /2);

            this.drawColor = drawColor;
            this.drawSize = drawSize;
        }

        public virtual void BeforeDraw()
        {

        }

        public virtual void AfterDraw()
        {

        }
    }
}
