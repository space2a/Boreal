using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;

namespace boreal.engine
{
    public class TileMap : Component
    {
        public Tile[] tiles { private set; get; }

        public int[] topTiles { private set; get; }
        public int[] rightTiles { private set; get; }
        public int[] leftTiles { private set; get; }
        public int[] bottomTiles { private set; get; }

        public int width { private set; get; }
        public int height { private set; get; }
        public int tilesSize { private set; get; }

        public TileMap[] neighborsTileMaps = new TileMap[4];

        private bool autoTile = true;

        public TileMap()
        {
        }

        public TileMap(int width, int height, int tilesSize, bool getNeighbors = true)
        {
            this.width = width;
            this.height = height;
            this.tilesSize = tilesSize;

            this.tiles = new Tile[width * height];

            this.topTiles = new int[height];
            this.rightTiles = new int[width];
            this.bottomTiles = new int[height];
            this.leftTiles = new int[width];

            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Tile tile = new Tile() { position = (new Vector2(x, y) * tilesSize), tileSize = tilesSize, tileMap = this, tileIndex = i };
                    tiles[i++] = tile;
                }
            }

            if (getNeighbors)
            {
                for (int t = 0; t < tiles.Length; t++)
                {
                    tiles[t].GetNeighbors();
                }
            }

            PutInAppropriateArrays();
        }

        protected override void Start() //attached to transform and gameObject
        {
            gameObject.currentScene.tileMapBatcher.tilemaps.Add(this);
        }

        internal override void DestroyComponent()
        {
            gameObject.currentScene.tileMapBatcher.tilemaps.Remove(this);
            tiles = new Tile[0];
        }

        internal TileMap GetNeighborTileMap(int pos)
        {
            if (pos == -1) return this;
            else return neighborsTileMaps[pos];
        }

        private void PutInAppropriateArrays()
        {
            int t = 0, r = 0, l = 0, b = 0;

            for (int x = 0; x < tiles.Length; x++)
            {
                if (tiles[x].position.X == 0)
                    leftTiles[t++] = x;
                else if (tiles[x].position.X == width - 1)
                    rightTiles[r++] = x;


                if (tiles[x].position.Y == 0)
                    bottomTiles[b++] = x;
                else if (tiles[x].position.Y == height - 1)
                    topTiles[l++] = x;
            }
        }

        public void BeginSetTiles()
        {
            autoTile = false;
        }

        public void EndSetTiles()
        {
            autoTile = true;
            for (int t = 0; t < tiles.Length; t++)
            {
                tiles[t].AutoTiling();
            }
        }

        public void SetTilemapNeighbor(TileMap tileMap, int position) //0 top 1 right 2 bottom 3 left
        {
            switch (position)
            {
                case 0:

                    break;

                case 1:

                    break;
            }
        }

        public bool SetTile(Tile tile, int index)
        {
            if (index > tiles.Length) return false;
            else if (index < 0) return false;

            tile.position = tiles[index].position;
            tile.tileSize = tilesSize;
            tile.tileMap = this;
            tile.tileIndex = tile.tileIndex;

            tile.neighborsIndex = tiles[index].neighborsIndex;

            tiles[index] = tile;

            if (autoTile)
            {
                for (int t = 0; t < tiles.Length; t++)
                {
                    tiles[t].AutoTiling();
                }
            }

            return true;
        }

        public bool SetTile(Tile tile, Vector2 position)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].position == position)
                {
                    return SetTile(tile, i);
                }
            }
            return false;
        }

        public Tile GetTile(Vector2 position)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i].position == position)
                {
                    return tiles[i];
                }
            }
            return null;
        }

        public Tile[] GetTiles(List<Vector2> positions, int startAt = 0, int max = 0)
        {
            Tile[] getTiles = new Tile[positions.Count];
            if (startAt < 0) startAt = 0;

            if (max == 0) max = tiles.Length;
            else if (max > tiles.Length) max = tiles.Length;

            for (int i = startAt; i < max; i++)
            {
                for (int p = 0; p < positions.Count; p++)
                {
                    if (positions[p] == null) continue;
                    if (positions[p].X == tiles[i].position.X && positions[p].Y == tiles[i].position.Y)
                    {
                        getTiles[p] = tiles[i];
                        positions[p] = null;
                        break;
                    }

                }
            }

            return getTiles;
        }


        public TileMap CloneEmpty()
        {
            TileMap tileMap = new TileMap(width, height, tilesSize, false);

            for (int i = 0; i < this.tiles.Length; i++)
            {
                //var t = Activator.CreateInstance(this.tiles[i].GetType(), null) as Tile;
                //t.tileMap = this.tiles[i].tileMap;
                //t.position = this.tiles[i].position;
                //t.tileSize = this.tiles[i].tileSize;
                //t.tileIndex = this.tiles[i].tileIndex;

                for (int n = 0; n < tileMap.tiles[i].neighborsIndex.Length; n++)
                {
                    if (this.tiles[i].neighborsIndex[n] == null) continue;
                    tileMap.tiles[i].neighborsIndex[n] = new TileNeighbor()
                    {
                        tileIndex = this.tiles[i].neighborsIndex[n].tileIndex,
                        tileMapIndex = this.tiles[i].neighborsIndex[n].tileMapIndex
                    };
                }
            }
            return tileMap;
        }

        internal override void Draw(Drawer spritesBatch)
        {
            if (transform == null) return;
            if (gameObject.currentScene.tileMapBatcher.isEnabled) return; //no need to draw, the batcher will do it

            spritesBatch.sprites.End();
            spritesBatch.Begin(spritesBatch.camera, false, false, SpriteSortMode.Texture);

            DrawTiles(spritesBatch);

            spritesBatch.End();
            spritesBatch.Begin(spritesBatch.camera, false);
        }

        internal void DrawTiles(Drawer spritesBatch)
        {
            foreach (var tile in tiles) //faster here than a for loop
            {
                tile.sprite?.Draw(spritesBatch, tile.position, transform, tilesSize);
            }
        }
    }

    public class Tile
    {
        internal TileMap tileMap;
        internal int tileSize;

        public Vector2 position;
        public Sprite sprite;

        public int tileIndex { internal set; get; }

        public TileNeighbor[] neighborsIndex = new TileNeighbor[8];

        internal void GetNeighbors()
        {
            var n = Get8Neighbors(this.tileIndex - 35, this.tileIndex + 35);

            for (int i = 0; i < 8; i++)
            {
                if (n[i] == null) continue;
                neighborsIndex[i] = new TileNeighbor() { tileIndex = n[i].tileIndex, tileMapIndex = -1 };
            }
        }

        public Tile[] GetDirectNeighbors()
        {
            return tileMap.GetTiles(new Vector2[]
            {
                new Vector2(position.X, position.Y + tileSize), //top
                new Vector2(position.X + (tileSize), position.Y), //right
                new Vector2(position.X, position.Y - tileSize), //bottom
                new Vector2(position.X - (tileSize), position.Y) //left
            }.ToList());
        }

        public Tile[] Get8Neighbors(int startAt = 0, int max = 0)
        {
            return tileMap.GetTiles(new Vector2[]
            {
                new Vector2(position.X - tileSize, position.Y + tileSize), //top left
                new Vector2(position.X, position.Y + tileSize), //top
                new Vector2(position.X + tileSize, position.Y + tileSize), //top right

                new Vector2(position.X - tileSize, position.Y), //left
                new Vector2(position.X + tileSize, position.Y), //right

                new Vector2(position.X - tileSize, position.Y - tileSize), //bottom left
                new Vector2(position.X, position.Y - tileSize), //bottom
                new Vector2(position.X + tileSize, position.Y - tileSize), //bottom right
            }.ToList(), startAt, max);
        }

        internal virtual void AutoTiling()
        {

        }

    }

    public class TileRule : Tile
    {
        public List<RuledTile> ruledTiles = new List<RuledTile>();
        public List<Texture2D> whitelist = new List<Texture2D>();

        internal override void AutoTiling()
        {

            for (int i = 0; i < ruledTiles.Count; i++)
            {
                var ruledTile = ruledTiles[i];
                int[] isCorrectTile = new int[8];

                for (int r = 0; r < ruledTile.tilingRule.Length; r++)
                {
                    Tile neighborTile = null;

                    if (neighborsIndex[r] == null || tileMap.GetNeighborTileMap(neighborsIndex[r].tileMapIndex) == null)
                    {
                        if (ruledTile.tilingRule[r] == 1)
                        {
                            isCorrectTile[r] = -1;
                        }
                        else if (ruledTile.tilingRule[r] == -1)
                        {
                            isCorrectTile[r] = -1;
                        }
                        continue;
                    }


                    var tiles = tileMap.GetNeighborTileMap(neighborsIndex[r].tileMapIndex)?.tiles;
                    if (tiles.Length <= 0) continue;

                    neighborTile = tiles[neighborsIndex[r].tileIndex];
                    if (neighborTile == null) continue;
                    bool result = false;

                    if (neighborTile.sprite == null)
                        result = false;
                    else
                    {
                        result = whitelist.Contains(neighborTile.sprite.texture2D);

                        if (!result)
                            result = neighborTile.sprite.texture2D.assetName == this.sprite.texture2D.assetName;
                    }

                    if (ruledTile.tilingRule[r] == 1)
                    {
                        if (result)
                            isCorrectTile[r] = 1;
                    }
                    else if (ruledTile.tilingRule[r] == -1)
                    {
                        if (!result)
                            isCorrectTile[r] = -1;
                    }
                    else
                        isCorrectTile[r] = 0;

                }

                bool isThisTile = true;
                for (int s = 0; s < isCorrectTile.Length; s++)
                {
                    if (isCorrectTile[s] != ruledTile.tilingRule[s]) { isThisTile = false; break; }
                }

                if (isThisTile)
                {
                    if (sprite != ruledTile.sprite)
                    {
                        //inform neighbors of the change
                        //for (int n = 0; n < neighborsIndex.Length; n++)
                        //{
                        //    if(neighborsIndex[n] != null)
                        //        tileMap.tiles[(int)neighborsIndex[n]].AutoTiling();
                        //}
                        //Console.WriteLine("update neighbors");
                    }
                    sprite = ruledTile.sprite;
                    return;
                    //Console.WriteLine("Valid : " + ruledTile.sprite + " n " + i);
                }
            }

            //Console.WriteLine("No valid rule");
        }
    }

    public class RuledTile
    {
        public Sprite sprite;

        public int[] tilingRule = new int[8] // -1 = NO | 0 = IGNORE | 1 = YES
        {
            0, 0, 0, //TOP LEFT TOP MIDDLE TOP RIGHT
            0,    0, //MIDDLE LEFT         MIDDLE RIGHT
            0, 0, 0 //BOTTOM LEFT BOTTOM MIDDLE BOTTOM RIGHT
        };

        public RuledTile(Sprite sprite)
        {
            this.sprite = sprite;
        }
    }


    public class TileNeighbor
    {
        public int tileMapIndex;
        public int tileIndex;
    }
}
