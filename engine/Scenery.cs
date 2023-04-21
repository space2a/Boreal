namespace boreal.engine
{

    public class Scenery
    {
        public Color clearingColor = Color.Black;
        public enum SceneryType
        {
            Colorized,
            Texturized,
            Gradient
        }

        public enum ScalingType
        {
            Camera,
            Rendering
        }

        public SceneryType sceneryType { get; internal set; }
        public ScalingType scalingType = ScalingType.Rendering;

        internal virtual void ApplyScenery(Sprites SpritesBatch, Camera camera)
        {

        }
    }

    public class ColorizedScenery : Scenery
    {
        public Color sceneryColor;

        public ColorizedScenery(Color color)
        {
            sceneryColor = color;
            sceneryType = SceneryType.Colorized;
        }

        public ColorizedScenery(Color color, ScalingType scalingType)
        {
            sceneryColor = color;
            sceneryType = SceneryType.Colorized;
            this.scalingType = scalingType;
        }

        internal override void ApplyScenery(Sprites SpritesBatch, Camera camera)
        {
            Launcher.core.GraphicsDevice.Clear(sceneryColor.color);
        }
    }

    public class GradientScenery : Scenery
    {
        public Gradient sceneryGradient;

        public GradientScenery(Gradient gradient)
        {
            sceneryGradient = gradient;
            sceneryType = SceneryType.Gradient;
        }

        public GradientScenery(Gradient gradient, ScalingType scalingType)
        {
            sceneryGradient = gradient;
            sceneryType = SceneryType.Gradient;
            this.scalingType = scalingType;
        }

        internal override void ApplyScenery(Sprites SpritesBatch, Camera camera)
        {
            if (scalingType == ScalingType.Rendering)
                SpritesBatch.shapes.DrawFilledRectangleGradient(Launcher.core.windowProfile.XNAboundningRectangle, sceneryGradient);
            else if (scalingType == ScalingType.Camera)
            {
                SpritesBatch.shapes.DrawFilledRectangleGradient(camera.XNABoundingRectangle, sceneryGradient);
            }
        }

    }

    public class TexturizedScenery : Scenery
    {
        public Texture2D sceneryTexture;

        public TexturizedScenery(Texture2D texture)
        {
            sceneryTexture = texture;
            sceneryType = SceneryType.Texturized;
        }

        public TexturizedScenery(Texture2D texture, ScalingType scalingType)
        {
            sceneryTexture = texture;
            sceneryType = SceneryType.Texturized;
            this.scalingType = scalingType;
        }

        internal override void ApplyScenery(Sprites SpritesBatch, Camera camera)
        {
            Launcher.core.GraphicsDevice.Clear(clearingColor.color);
            //spriteBatch.Begin();
            if(scalingType == ScalingType.Rendering)
                SpritesBatch.Draw(sceneryTexture.texture2D, Launcher.core.windowProfile.XNAboundningRectangle, Color.White.color);
            else if(scalingType == ScalingType.Camera)
            {
                SpritesBatch.Draw(sceneryTexture.texture2D, camera.XNABoundingRectangle, boreal.engine.Color.White.color);
            }
        }
    }
}