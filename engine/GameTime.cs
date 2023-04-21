using System;

namespace boreal.engine
{
    public class GameTime
    {

        public TimeSpan ElapsedGameTime;
        public float ElapsedTotalSeconds;

        public GameTime(Microsoft.Xna.Framework.GameTime gameTime) 
        {
            ElapsedGameTime = gameTime.ElapsedGameTime;
            ElapsedTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
