namespace boreal.engine
{
    public class Light : Component
    {
        public Texture2D lightTexture;
        public Vector2 scale = Vector2.One;

        public Color color = Color.White;

        public bool drawTexture = true;

        protected override void Start()
        {
            gameObject.currentScene.sceneLightmap.AddLight(this);
        }

        internal override void DestroyComponent()
        {
            gameObject.currentScene.sceneLightmap.RemoveLight(this);
        }
    }

    public class LightPoint : Light
    {
        public LightPoint()
        {
            lightTexture = LoadEmbeddedResources.LoadTexture("light.png", "images");
        }

        public LightPoint(Vector2 scale, Color color)
        {
            lightTexture = LoadEmbeddedResources.LoadTexture("light.png", "images");
            this.scale = scale;
            this.color = color;
        }
    }

    public class AmbientLight : Light
    {
        public AmbientLight()
        {
            lightTexture = new Texture2D(Launcher.windowProfile.renderedResolution.width, Launcher.windowProfile.renderedResolution.height);
            color = Color.White;
        }
    }
}
