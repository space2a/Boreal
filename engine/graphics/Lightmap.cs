using System.Collections.Generic;

namespace boreal.engine
{
    public class Lightmap
    {
        public AmbientLight ambientLight;
        public List<Light> lights { get; private set; }

        public Lightmap()
        {
            lights = new List<Light>();
            ambientLight = new AmbientLight() { drawTexture = false };
            ambientLight.PreStart();
        }

        internal void AddLight(Light light)
        {
            lights.Add(light);
            light.PreStart();
        }

        internal void RemoveLight(Light light)
        {
            lights.Remove(light);
        }

    }
}