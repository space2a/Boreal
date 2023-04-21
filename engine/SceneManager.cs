using System.Collections.Generic;

namespace boreal.engine
{
    public static class SceneManager
    {
        internal static List<Scene> scenes = new List<Scene>();

        public static Scene currentScene = null;
        public static Scene loadingScene { get; internal set; }

        public static void LoadScene(string sceneName)
        {
            currentScene = scenes.Find(x => x.sceneName == sceneName);
            Launcher.core.LoadScene(currentScene);
        }

        public static void LoadScene(Scene scene)
        {
            currentScene = scene;
            Launcher.core.LoadScene(currentScene);
        }

        public static void SetLoadingScene(Scene loadingScene)
        {
            Launcher.core.LoadLoadingScene(loadingScene);
        }

        public static bool DeclareScene(Scene scene)
        {
            return scene.CreateScene(scene.sceneName);
        }

        public static bool DeclareScene(Scene scene, string sceneName)
        {
            return scene.CreateScene(sceneName);
        }

        public static void ImportGameObjectsFromScene(Scene sourceScene, Scene destinationScene)
        {
            destinationScene.AddGameObjects(sourceScene.gameObjects);
        }

    }
}