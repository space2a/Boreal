using System.IO;
using System.Reflection;

namespace boreal.engine
{
    internal static class LoadEmbeddedResources
    {
        public static byte[] LoadResource(string resourceName, string folderName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = "boreal.embedded." + folderName + "." + resourceName;

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                byte[] d = new byte[stream.Length];
                stream.Read(d, 0, d.Length);
                stream.Close();
                return d;
            }
        }

        public static Stream LoadResourceStream(string resourceName, string folderName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = "boreal.embedded." + folderName + "." + resourceName;
            return assembly.GetManifestResourceStream(resourcePath);
        }

        public static Texture2D LoadTexture(string resourceName, string folderName)
        {
            return new Texture2D(LoadResourceStream(resourceName, folderName)) { assetName = resourceName.Remove(resourceName.LastIndexOf(".")) };
        }

    }
}
