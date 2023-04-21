using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;

using boreal.engine;

using System;

using GameTime = Microsoft.Xna.Framework.GameTime;

namespace boreal
{
    internal class Core : Game
    {
        internal CollisionComponent _collisionComponent;

        internal GraphicsDeviceManager _graphics;

        internal Sprites spritesBatch;

        internal bool loadingScene = true;

        public Scene loadedScene;

        public WindowProfile windowProfile { get; private set; }
        public Screen screen;

        private Camera noCameraCam;
        private Camera getMainCamera
        {
            get
            {
                if (loadedScene == null)
                    return noCameraCam;
                else if (loadedScene.sceneCamera != null)
                    return loadedScene.sceneCamera;
                else if (SceneManager.loadingScene != null && SceneManager.loadingScene.sceneCamera != null && loadingScene)
                    return SceneManager.loadingScene.sceneCamera;
                else
                    return noCameraCam;
            }
        }

        public Core(WindowProfile windowProfile)
        {
            this.windowProfile = windowProfile;


            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            IsMouseVisible = true;

            CreateCollisionComponent();

            this.Deactivated += Core_Deactivated;
            this.Activated += Core_Activated;

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void Core_Activated(object sender, EventArgs e)
        {
            Inputs.MouseState.safeLock = true;
        }

        private void Core_Deactivated(object sender, EventArgs e)
        {
            Inputs.MouseState.safeLock = false;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;

            //changing the window size
            _graphics.PreferredBackBufferWidth = (int)windowProfile.windowResolution.width;
            _graphics.PreferredBackBufferHeight = (int)windowProfile.windowResolution.height;
            _graphics.ApplyChanges();

            Window.AllowAltF4 = windowProfile.authorizeALTF4;
            Window.AllowUserResizing = windowProfile.authorizeResizing;
            Window.AllowUserResizing = true;
            Window.Title = windowProfile.windowTitle;
            Window.IsBorderless = windowProfile.windowState == WindowProfile.WindowState.Borderless;

            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            screen = new Screen(windowProfile.renderedResolution.width, windowProfile.renderedResolution.height);

            spritesBatch = new Sprites(this);

            noCameraCam = new Camera();

            base.Initialize();
        }

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

        private void CallUpdates(engine.GameTime gt, Camera cam, Scene scene)
        {
            scene.CallUpdates(gt);
            cam.canvas?.CallUpdate(gt);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!Launcher.GetCoreAndBackgroundState()) return;

            var gt = new engine.GameTime(gameTime);
            Camera cam = getMainCamera;

            Inputs.Update();
            Inputs.MouseState.UpdateScreenPosition(screen, cam);

            try
            {
                _collisionComponent.Update(gameTime); //update the collision.
            }
            catch (Exception)
            {
                Console.WriteLine("collision engine bug");
            }

            if (loadedScene == null && SceneManager.loadingScene != null)
            {
                CallStartsIfNeeded(SceneManager.loadingScene);
                CallUpdates(gt, cam, SceneManager.loadingScene);
                base.Update(gameTime); 
                return;
            }

            if (loadingScene) return;

            CallStartsIfNeeded(loadedScene);

            CallUpdates(gt, cam, loadedScene);

            base.Update(gameTime);
        }

        private void CallDraws(Sprites spritesBatch, Scene scene, out Scene currentScene)
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



        protected override void Draw(GameTime gameTime)
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

            base.Draw(gameTime);
            //Console.WriteLine((DateTime.Now - start).TotalMilliseconds + " ms draw");
        }

        private void EndDrawBatchs(Scene scene, Camera cam)
        {
            engine.Debugging.Draw(spritesBatch);
            spritesBatch.End();

            //draws UIs
            if(scene != null)
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

        public void LoadScene(Scene scene)
        {
            DateTime dateTime = DateTime.Now;
            loadingScene = true;
            CreateCollisionComponent();

            if (loadedScene != null)
                loadedScene.Destroy();

            loadedScene = null;


            if (scene == null) { throw new Exception("no scene to load content from."); }
            Console.WriteLine("Core.LoadScene(..):" + scene.sceneName + " with : " + scene.gameObjects.Count + "bad gobj(s)");

            loadedScene = scene;
            scene.OnCreationCall(dateTime); // <--- OnCreationCall() will set loadingScene to false when done.
        }

        public void LoadLoadingScene(Scene loadingScene)
        {
            LoadContent();

            if (SceneManager.loadingScene != null)
                SceneManager.loadingScene.Destroy();

            SceneManager.loadingScene = loadingScene;
            SceneManager.loadingScene.OnCreation();

        }

        internal void CreateCollisionComponent()
        {
            _collisionComponent = new CollisionComponent(
                new RectangleF((windowProfile.renderedResolution.width * 10) /2 * -1, (windowProfile.renderedResolution.height * 10) /2 * -1,
                windowProfile.renderedResolution.width * 10, windowProfile.renderedResolution.height * 10));
        }
    }

}