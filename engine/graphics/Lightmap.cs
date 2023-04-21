using System.Collections.Generic;

namespace boreal.engine
{
    public class Lightmap
    {
        public AmbientLight ambientLight = new AmbientLight() { drawTexture = false };
        public List<Light> lights { get; private set; }

        public Lightmap()
        {
            lights = new List<Light>();
        }

        internal void AddLight(Light light)
        {
            lights.Add(light);
        }

        internal void RemoveLight(Light light)
        {
            lights.Remove(light);
        }

    }
}