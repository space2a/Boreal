using boreal.engine;

using System;

namespace boreal
{
    public class DefaultCore : CoreInstance
    {

        //Start...
        private void CallStartsIfNeeded(Scene scene)
        {
            if (!scene.startedCalled)
            {
                scene.startedCalled = true;
                for (int i = 0; i < scene.gameObjects.Count; i++)
                    scene.gameObjects[i].Start();

                scene.sceneCamera?.canvas?.CallStart();
            }
        }

        //Updates...
        private void CallUpdates(engine.GameTime gt, Camera cam, Scene scene)
        {
            scene.CallUpdates(gt);
            cam.canvas?.CallUpdate(gt);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            Camera cam = getMainCamera;

            UpdateInputs(cam);

            if (loadedScene == null && SceneManager.loadingScene != null)
            {
                CallStartsIfNeeded(SceneManager.loadingScene);
                CallUpdates(gameTime, cam, SceneManager.loadingScene);
                return;
            }

            if (loadingScene) return;

            CallStartsIfNeeded(loadedScene);

            CallUpdates(gameTime, cam, loadedScene);
        }



        //Draws...
        private void CallDraws(Drawer spritesBatch, Scene scene, out Scene currentScene)
        {
            currentScene = scene;
            currentScene.Draw(spritesBatch);
        }

        public void StartDrawBatchs(out Camera cam)
        {
            cam = getMainCamera;

            screen.Set();

            loadedScene?.BeforeDraw(spritesBatch, cam);

            spritesBatch.Begin(cam, false);
            spritesBatch.shapes.Begin(cam);
        }

        protected override void OnDraw(GameTime gameTime)
        {
            DateTime start = DateTime.Now;
            if (!Launcher.GetCoreAndBackgroundState()) return;

            Scene currentScene = null;
            StartDrawBatchs(out Camera cam);

            if (cam == noCameraCam && loadedScene != null)
            { ScreenErrors.NoMainCamera(spritesBatch); EndDrawBatchs(currentScene, cam); return; }

            if (loadingScene || !loadedScene.startedCalled)
            {
                if (SceneManager.loadingScene != null)
                    CallDraws(spritesBatch, SceneManager.loadingScene, out currentScene);
                else { ScreenErrors.LoadingSceneNull(spritesBatch); EndDrawBatchs(currentScene, cam); return; }
            }
            else if (loadedScene != null && loadedScene.startedCalled)
                CallDraws(spritesBatch, loadedScene, out currentScene);

            EndDrawBatchs(currentScene, cam);
        }

        private void EndDrawBatchs(Scene scene, Camera cam)
        {
            engine.Debugging.Draw(spritesBatch);
            spritesBatch.End();

            //draws UIs
            if (scene != null)
            {
                spritesBatch.BeginUI();
                scene.DrawUI(spritesBatch);
                spritesBatch.End(); //ends both ui and non-ui batchs.
            }

            spritesBatch.shapes.isDrawingInCanvas = true;
            spritesBatch.shapes.SetMatrixToDefault();

            screen.UnSet(spritesBatch, cam);
            spritesBatch.shapes.End();
            spritesBatch.shapes.isDrawingInCanvas = false;
            screen.Present(spritesBatch, scene, cam);
        }



        //Scenes loading...
        public override void LoadScene(Scene scene)
        {
            DateTime dateTime = DateTime.Now;
            loadingScene = true;

            if (loadedScene != null)
                loadedScene.Destroy();

            loadedScene = null;


            if (scene == null) { throw new Exception("no scene to load content from."); }
            Console.WriteLine("Core.LoadScene(..):" + scene.sceneName + " with : " + scene.gameObjects.Count + "bad gobj(s)");

            loadedScene = scene;
            scene.OnCreationCall(dateTime); // <--- OnCreationCall() will set loadingScene to false when done.
        }

        public override void LoadLoadingScene(Scene loadingScene)
        {
            if (SceneManager.loadingScene != null)
                SceneManager.loadingScene.Destroy();

            SceneManager.loadingScene = loadingScene;
            SceneManager.loadingScene.OnCreation();
        }
    }
}
