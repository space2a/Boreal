using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public class Canvas
    {
        public List<GameObject> gameObjects { get; private set; }

        public void Add(GameObject gobj)
        {
            if (gameObjects == null) gameObjects = new List<GameObject>();
            gobj.isInCanvas = true;
            Console.WriteLine(gobj.isInCanvas);

            if (starts)
                gobj.Start();

            gameObjects.Add(gobj);
        }

        private bool starts = false;
        public void CallStart()
        {
            if (starts) return;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Start();
            }
            starts = true;
        }

        internal void CallDrawUI(Sprites spriteBatch)
        {
            if (gameObjects == null) return;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].DrawUI(spriteBatch);
                //Console.WriteLine("calling draw ui on " + uiComponents[i].GetType());
            }
        }

        //internal void CallDraw(Sprites spriteBatch)
        //{
        //    if (gameObjects == null) return;
        //    for (int i = 0; i < gameObjects.Count; i++)
        //    {
        //        gameObjects[i].Draw(spriteBatch);
        //    }
        //}

        internal void CallUpdate(GameTime gameTime)
        {
            if (gameObjects == null) return;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].PreUpdate(gameTime);
            }
        }

    }
}
