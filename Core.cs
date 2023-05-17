using boreal.engine;
using boreal.engine.graphics;

using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;

using System;

using GameTime = Microsoft.Xna.Framework.GameTime;

namespace boreal
{
    internal class Core : Game
    {
        public WindowProfile windowProfile { get; private set; }

        public CoreInstance instance { get; private set; }

        internal CollisionComponent _collisionComponent;

        internal GraphicsDeviceManager _graphics;

        internal Drawer spritesBatch;

        internal Screen screen;

        public Core(WindowProfile windowProfile, CoreInstance coreInstance)
        {
            this.windowProfile = windowProfile;
            this.instance = coreInstance;
            this.instance.core = this;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            IsMouseVisible = true;

            CreateCollisionComponent();

            this.Deactivated += delegate (object sender, EventArgs e) { Inputs.MouseState.safeLock = false; };
            this.Activated += delegate (object sender, EventArgs e) { Inputs.MouseState.safeLock = true; };
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

            spritesBatch = new Drawer(this);

            instance.noCameraCam = new Camera();

            base.Initialize();
        }

        internal void CreateCollisionComponent()
        {
            _collisionComponent = new CollisionComponent(
                new RectangleF((windowProfile.renderedResolution.width * 10) /2 * -1, (windowProfile.renderedResolution.height * 10) /2 * -1,
                windowProfile.renderedResolution.width * 10, windowProfile.renderedResolution.height * 10));
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                _collisionComponent.Update(gameTime); //update the collision.
            }
            catch (Exception)
            {
                Console.WriteLine("collision engine bug");
            }

            instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            instance.Draw(gameTime);
            base.Draw(gameTime);
        }
    }

    public class CoreInstance
    {
        internal Core core;

        public Scene loadedScene;

        public bool loadingScene = true;

        internal Screen screen
        {
            get
            {
                return core.screen;
            }
        }

        internal Drawer spritesBatch
        {
            get
            {
                return core.spritesBatch;
            }
        }

        public EssentialDrawer essentialDrawer
        {
            get
            {
                return core.spritesBatch.essentialDrawer;
            }
        }

        protected Camera getMainCamera
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

        internal Camera noCameraCam;

        internal void Update(GameTime gameTime)
        {
            if (!Launcher.GetCoreAndBackgroundState()) return;

            var gt = new engine.GameTime(gameTime);

            if (OnUpdateStart())
            {
                OnUpdate(gt);
                OnUpdateEnd();
            }
        }

        //Updates...

        protected virtual bool OnUpdateStart() { return true; }

        protected virtual void OnUpdate(engine.GameTime gameTime) { }

        protected virtual void OnUpdateEnd() { }

        //Draws...
        protected virtual bool OnDrawStart(out Camera cam)
        {
            cam = getMainCamera;
            return true;
        }

        internal void Draw(GameTime gameTime)
        {
            engine.GameTime gt = new engine.GameTime(gameTime);

            if(OnDrawStart(out Camera cam))
            {
                OnDraw(gt);
                OnDrawEnd(loadedScene, cam);
            }

            //Console.WriteLine((DateTime.Now - start).TotalMilliseconds + " ms draw");
        }

        protected virtual void OnDraw(engine.GameTime gameTime) { }

        protected virtual void OnDrawEnd(Scene scene, Camera cam) { }

        public virtual void LoadScene(Scene scene) { }

        public virtual void LoadLoadingScene(Scene loadingScene) { }

        public void BeginDraw(Camera cam, bool immediate = false)
        {
            Microsoft.Xna.Framework.Graphics.SpriteSortMode ssm = Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred;
            if (immediate) ssm = Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate;
            spritesBatch.Begin(cam, false, spriteSortMode: ssm);
            spritesBatch.shapes.Begin(cam);
        }

        public void BeginDrawUI()
        {
            spritesBatch.End();
            spritesBatch.shapes.isDrawingInCanvas = true;
            spritesBatch.shapes.SetMatrixToDefault();
            spritesBatch.BeginUI();
        }

        public void DrawCameraCanvas(Camera cam)
        {
            cam.DrawCanvas(spritesBatch);
        }

        public void Draw(Object obj, Camera cam, bool UI = false)
        {
            switch (obj)
            {
                case GameObject g:
                    if (!UI)
                        g.Draw(spritesBatch);
                    else
                        g.DrawUI(spritesBatch);
                    break;
                case Component c:
                    if(!UI)
                        c.PreDraw(spritesBatch);
                    else
                        c.PreDrawUI(spritesBatch);
                    break;
                case Scenery s:
                    s.ApplyScenery(spritesBatch, cam);
                    break;
                default:
                    spritesBatch.DrawString(FontManager.defaultFont.font, obj.ToString(), new Microsoft.Xna.Framework.Vector2(0, 0), Microsoft.Xna.Framework.Color.Black);
                    break;
            }
        }

        public void EndDraw()
        {
            engine.Debugging.Draw(spritesBatch);
            spritesBatch.End();
            spritesBatch.shapes.End();
            spritesBatch.shapes.isDrawingInCanvas = false;
        }

        public void ExecuteDrawActionsNow()
        {
            spritesBatch.ExecuteDrawActions();
        }

        public void Update(Object obj, engine.GameTime gameTime)
        {
            switch (obj)
            {
                case GameObject g:
                    g.PreUpdate(gameTime);
                    break;
                case Component c:
                    c.PreUpdate(gameTime);
                    break;
            }
        }

        public void UpdateInputs(Camera cam)
        {
            Inputs.Update();
            Inputs.MouseState.UpdateScreenPosition(screen, cam);
        }
    }


}