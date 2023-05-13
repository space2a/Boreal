using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace boreal.engine.graphics
{
    public class TileMapBatcher
    {
        public bool isEnabled = false;
        public List<TileMap> tilemaps { get; internal set; }

        public TileMapBatcher() { tilemaps = new List<TileMap>(); }

        internal void Draw(Drawer spritesBatch)
        {
            if (!isEnabled) return;
            DateTime globalStart = DateTime.Now;
            spritesBatch.Begin(spritesBatch.camera, false, false, SpriteSortMode.Texture);

            for (int i = 0; i < tilemaps.Count; i++)
            {
                tilemaps[i].DrawTiles(spritesBatch);
            }

            spritesBatch.End();
            //Console.WriteLine((DateTime.Now - globalStart).TotalMilliseconds + " ms total batch");
        }
    }
}
