using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace boreal.engine
{

    public class Sprite
    {
        public Texture2D texture2D { get; protected set; }
        public Color color = Color.White;

        internal SpriteEffects _flip = SpriteEffects.FlipVertically;
        public Flip flip
        {
            get
            {
                return GetFlip(_flip);
            }
            set
            {
                _flip = GetFlip(value);
            }
        }

        public boreal.engine.Vector2 origin = boreal.engine.Vector2.Zero;

        public int orderBy = 0;

        internal virtual void Draw(Drawer spritesBatch, Transform transform)
        {

        }

        internal virtual void Draw(Drawer spritesBatch, Vector2 position, Transform transform, int baseScaling)
        {

        }

        internal virtual void Draw(Drawer spritesBatch, Vector2 position, Vector2 scaling, Vector2 origin, Transform transform, Color color)
        {

        }

        public enum Flip
        {
            None,
            FlipVertically,
            FlipHorizontally
        }
        
        public void CalculateOrigin()
        {
            origin = new engine.Vector2(texture2D.width /2, texture2D.height /2);
        }

        internal SpriteEffects GetFlip(Flip flip)
        {
            Console.WriteLine("SFLIP");
            switch (flip)
            {
                case Flip.None:
                    return SpriteEffects.FlipVertically;
                case Flip.FlipVertically:
                    return SpriteEffects.None;
                case Flip.FlipHorizontally:
                    return SpriteEffects.FlipHorizontally;
            }
            return SpriteEffects.FlipVertically;
        }

        internal Flip GetFlip(SpriteEffects spriteEffects)
        {
            Console.WriteLine("FFLIP");
            switch (spriteEffects)
            {
                case SpriteEffects.FlipVertically:
                    return Flip.None;
                case SpriteEffects.None:
                    return Flip.FlipVertically;
                case SpriteEffects.FlipHorizontally:
                    return Flip.FlipHorizontally;
            }
            return Flip.None;
        }

    }

    public class SingleSprite : Sprite
    {
        public SingleSprite(Texture2D texture2D)
        {
            this.texture2D = texture2D;
            CalculateOrigin();
        }

        public SingleSprite(Texture2D texture2D, Color color)
        {
            this.texture2D = texture2D;
            this.color = color;
            CalculateOrigin();
        }


        internal override void Draw(Drawer spritesBatch, Transform transform)
        {
            spritesBatch.Draw(texture2D.texture2D, null, origin.xnaV2,
                (transform.position).xnaV2,
                transform.rotation, transform.scale.xnaV2, color, _flip, orderBy);
        }

        internal override void Draw(Drawer spritesBatch, Vector2 position, Transform transform, int baseScaling)
        {
            spritesBatch.Draw(texture2D.texture2D, null, Microsoft.Xna.Framework.Vector2.Zero,
                (transform.position + position).xnaV2,
                transform.rotation, transform.scale.xnaV2, color, _flip, orderBy);
        }

        internal override void Draw(Drawer spritesBatch, Vector2 position, Vector2 scaling, Vector2 origin, Transform transform, Color color)
        {
            spritesBatch.Draw(texture2D.texture2D, null, origin.xnaV2,
                position.xnaV2,
                transform.rotation, scaling.xnaV2, color, _flip, orderBy);
        }
    }

    public class MultipleSprite : Sprite
    {
        public int slicedBy { get; private set; }

        private MultipleSpriteCoords[] coords;

        public int currentSpriteIndex = 0;

        public int maxSpriteIndex { get; private set; }

        public MultipleSprite(Texture2D texture2D, int slicedBy)
        {
            this.texture2D = texture2D;
            this.slicedBy = slicedBy;

            origin = new engine.Vector2(slicedBy /2, slicedBy /2);
            Slice();
        }

        public MultipleSprite(Texture2D texture2D, int slicedBy, Color color)
        {
            this.texture2D = texture2D;
            this.slicedBy = slicedBy;
            this.color = color;

            origin = new engine.Vector2(slicedBy /2, slicedBy /2);
            Slice();
        }

        private void Slice()
        {
            int w = texture2D.texture2D.Width;
            int h = texture2D.texture2D.Height;

            coords = new MultipleSpriteCoords[(w / slicedBy) * (h / slicedBy)];

            int lastX = 0;
            lastX -= slicedBy;
            int lastY = 0;
            int i = 0;
            
            while (i != coords.Length)
            {
                int x = lastX + slicedBy;
                int y = lastY;
                lastX = x;
                lastY = y;

                if (x >= w) { lastY += slicedBy; lastX = 0; lastX -= slicedBy; continue; }

                coords[i++] = new MultipleSpriteCoords() { x = x, y = y };
                maxSpriteIndex++;
            }
        }

        public MultipleSprite this[int index]
        {
            get { if (index < maxSpriteIndex && index >= 0) currentSpriteIndex = index; return this; }
        }

        internal override void Draw(Drawer spritesBatch, Transform transform)
        {
            spritesBatch.Draw(texture2D.texture2D,
                new Microsoft.Xna.Framework.Rectangle(coords[currentSpriteIndex].x, coords[currentSpriteIndex].y, slicedBy, slicedBy),
                origin.xnaV2, (transform.position).xnaV2,
                transform.rotation,
                transform.scale.xnaV2, color, _flip, orderBy);
        }

        internal override void Draw(Drawer spritesBatch, Vector2 position, Transform transform, int baseScaling)
        {
            spritesBatch.Draw(texture2D.texture2D,
                new Microsoft.Xna.Framework.Rectangle(coords[currentSpriteIndex].x, coords[currentSpriteIndex].y, slicedBy, slicedBy),
                Microsoft.Xna.Framework.Vector2.Zero, (transform.position + position).xnaV2,
                transform.rotation,
                transform.scale.xnaV2, color, _flip, orderBy);
        }

        internal override void Draw(Drawer spritesBatch, Vector2 position, Vector2 scaling, Vector2 origin, Transform transform, Color color)
        {
            spritesBatch.Draw(texture2D.texture2D,
                new Microsoft.Xna.Framework.Rectangle(coords[currentSpriteIndex].x, coords[currentSpriteIndex].y, slicedBy, slicedBy),
                origin.xnaV2, position.xnaV2,
                transform.rotation,
                scaling.xnaV2, color, _flip, orderBy);
        }
    }

    public class RegionSprite : Sprite
    {
        public Rectangle region;
        internal Rectangle destination = null;

        public RegionSprite(Texture2D texture2D)
        {
            this.texture2D = texture2D;
            region = new engine.Rectangle(0, 0, texture2D.width, texture2D.height);
            CalculateOrigin();
        }

        public RegionSprite(Texture2D texture2D, Rectangle region)
        {
            this.texture2D = texture2D;
            this.region = region;
            CalculateOrigin();
        }

        internal override void Draw(Drawer spritesBatch, Transform transform)
        {
            if (region == null) return;
            spritesBatch.Draw(texture2D.texture2D, region.rect, origin.xnaV2,
                            (transform.position).xnaV2,
                            transform.rotation, transform.scale.xnaV2, color, _flip, orderBy);
        }

        internal override void Draw(Drawer spritesBatch, Vector2 position, Transform transform, int baseScaling)
        {
            if(destination == null)
            {
                var pos = (transform.position + position);
                destination = new Rectangle((int)pos.X * (int)transform.scale.X, (int)pos.Y *(int)transform.scale.Y, baseScaling * (int)transform.scale.X, baseScaling * (int)transform.scale.Y);
            }

            spritesBatch.sprites.Draw(texture2D.texture2D, destination.rect, region.rect, color.color, 0f, Microsoft.Xna.Framework.Vector2.Zero, _flip, orderBy);

        }

        internal override void Draw(Drawer spritesBatch, Vector2 position, Vector2 scaling, Vector2 origin, Transform transform, Color color)
        {
            spritesBatch.Draw(texture2D.texture2D, region.rect, origin.xnaV2,
                            position.xnaV2,
                            transform.rotation, scaling.xnaV2, color, _flip, orderBy);
        }

    }

    internal class MultipleSpriteCoords
    {
        public int x, y;
    }

}
