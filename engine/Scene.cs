using boreal.engine.graphics;

using System;
using System.Collections.Generic;
using System.Threading;

namespace boreal.engine
{
    public class Scene
    {
        public string sceneName = "";
        public List<GameObject> gameObjects = new List<GameObject>();

        public Scenery sceneScenery = new ColorizedScenery(Color.Black);

        public Camera sceneCamera = null;

        public Lightmap sceneLightmap = new Lightmap();

        public TileMapBatcher tileMapBatcher = new TileMapBatcher();

        internal bool startedCalled = false;

        public delegate void GameObjectModification(Scene scene, GameObject gameObject);
        public event GameObjectModification OnAddGameObject;

        private bool creationCall = false;

        public Scene()
        {
            
        }

        public Scene(string sceneName)
        {
            CreateScene(sceneName);
        }
       
        public virtual void OnCreation()
        {

        }

        public void OnCreationCall(DateTime dateTime)
        {
            if (creationCall) throw new Exception("OnCreationCall already called.");
            creationCall = true;
            int bad_gobjs = gameObjects.Count;
            new Thread(() =>
            {
                OnCreation();
                Launcher.core.instance.loadingScene = false;
                Console.WriteLine("scene loaded in " + (DateTime.Now - dateTime).TotalSeconds + "s with " +
                (gameObjects.Count - bad_gobjs) + " properly added gobjs.");
            }).Start();
        }

        public GameObject AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            OnAddGameObject?.Invoke(this, gameObject);
            if (startedCalled)
                gameObject.Start();

            return gameObject;
        }

        public void AddGameObjects(params GameObject[] gameObjects)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                AddGameObject(gameObjects[i]);
            }
        }

        public void AddGameObjects(List<GameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                AddGameObject(gameObjects[i]);
            }
        }

        public void InsertGameObject(GameObject gameObject, int index)
        {
            gameObjects.Insert(index, gameObject);
            if (startedCalled)
                gameObject.Start();
        }

        public bool CreateScene(string sceneName)
        {
            if (SceneManager.scenes.FindIndex(x => x.sceneName == sceneName) != -1)
            { throw new Exception("A scene with the same name already exists."); }
            this.sceneName = sceneName;
            SceneManager.scenes.Add(this);
            Console.WriteLine("new scene..." + sceneName);
            return true;
        }

        public GameObject GetGameObjectByName(string name)
        {
            return gameObjects.Find(x => x.name == name);
        }

        public GameObject[] GetGameObjectsByName(string name)
        {
            return gameObjects.FindAll(x => x.name == name).ToArray();
        }

        public GameObject GetGameObjectByTag(string tag)
        {
            return gameObjects.Find(x => x.tag == tag);
        }

        public GameObject[] GetGameObjectsByTag(string tag)
        {
            return gameObjects.FindAll(x => x.tag == tag).ToArray();
        }

        public void Destroy()
        {
            if (gameObjects != null)
                for (int i = 0; i < gameObjects.Count; i++)
                    gameObjects[i].PreDestroy();

            SceneManager.scenes.Remove(this);
        }

        internal void CallUpdates(GameTime gt)
        {
            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].PreUpdate(gt);
        }

        internal void BeforeDraw(Drawer spritesBatch, Camera cam)
        {
            sceneScenery?.ApplyScenery(spritesBatch, cam);
            tileMapBatcher?.Draw(spritesBatch);
        }

        internal void Draw(Drawer spritesBatch)
        {
            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Draw(spritesBatch);
        }

        internal void DrawUI(Drawer spritesBatch)
        {
            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].DrawUI(spritesBatch);
        }

    }
}
