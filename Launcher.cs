using System;
using System.IO;
using System.Reflection;

using boreal.engine;


using Microsoft.Xna.Framework;

using Point = boreal.engine.Point;
using Rectangle = boreal.engine.Rectangle;
using Vector2 = boreal.engine.Vector2;

namespace boreal
{
    public static class Launcher
    {
        internal static Core core;
        internal static bool launched = false;
        
        internal static CoreState coreState = CoreState.Stopped;
        internal static CoreState coreStateBackground = CoreState.Playing;

        public enum CoreState
        {
            Playing,
            Paused,
            Stopped
        }

        public static WindowProfile windowProfile
        {
            get
            {
                return core.windowProfile;
            }
        }

        public static void Ini(WindowProfile windowProfile, bool setWorkingDirectory = true)
        {
            core = new Core(windowProfile);
            if (setWorkingDirectory)
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
        }

        public static void Launch()
        {
            if(core == null) { throw new Exception("Call Ini() before calling this function."); }
            if (launched) throw new Exception("boreal already started.");
            launched = true;
            windowProfile.Initialize();
            coreState = CoreState.Playing;
            core.Run();

            if(windowProfile.startPosition == WindowProfile.StartPositionType.Manual)
                core.Window.Position = new Microsoft.Xna.Framework.Point(windowProfile.startPositionManual.X, windowProfile.startPositionManual.Y);
        }

        public static void WaitForInitialized()
        {
            if (core == null) { throw new Exception("Call Ini() before calling this function."); }
            while (!core.IsActive) { }
        }

        public static CoreState GetCoreState()
        {
            return coreState;
        }

        internal static bool GetCoreAndBackgroundState()
        {
            return coreState == CoreState.Playing && coreStateBackground == CoreState.Playing;
        }

        public static void Resume()
        {
            coreState = CoreState.Playing;
        }

        public static void Pause()
        {
            coreState = CoreState.Paused;
        }

        public static void Exit()
        {
            core.Exit();
        }

    }

    public class WindowProfile
    {
        public event EventHandler WindowActivated, WindowDeactivated, Exiting;
        public delegate void fileDrop(string[] files);
        public event fileDrop FileDrop;
        public delegate void windowResolutionChanged(Size2 newResolution, Size2 oldResolution);
        public event windowResolutionChanged WindowResolutionChanged;


        public Size2 windowResolution = new Size2(1920, 1080);
        public Size2 renderedResolution = new Size2(1240, 720);

        public string windowTitle;

        public bool authorizeResizing = false;
        public bool authorizeALTF4 = false;

        public bool runInBackground = false;

        public WindowState windowState = WindowState.Windowed;
        public StartPositionType startPosition = StartPositionType.CenterOwner;

        public Point windowPosition
        {
            get
            {
                if (Launcher.core != null)
                    return new Point(Launcher.core.Window.Position.X, Launcher.core.Window.Position.Y);
                else return new Point(0, 0);
            }
            set
            {
                if (Launcher.core != null)
                    new Microsoft.Xna.Framework.Point(value.X, value.Y);
            }
        }

        public Point startPositionManual = new engine.Point(0, 0);

        private Size2 oldWindowResolution;

        public Rectangle boundingRectangle { get; private set; }
        internal Microsoft.Xna.Framework.Rectangle XNAboundningRectangle { get; private set; }
        
        public enum WindowState
        {
            Windowed,
            Borderless,
            Fullscreen,
            FullscreenBorderless
        }

        public enum StartPositionType
        {
            Manual,
            CenterOwner
        }

        public WindowProfile() { }

        public WindowProfile(Size2 windowResolution, Size2 renderedResolution)
        {
            this.windowResolution = windowResolution;
            this.renderedResolution = renderedResolution;
        }

        public WindowProfile(Size2 windowResolution, Size2 renderedResolution, string windowTitle, bool authorizeResizing, bool authorizeALTF4, bool runInBackground, WindowState windowState)
        {
            this.windowResolution = windowResolution;
            this.renderedResolution = renderedResolution;
            this.windowTitle = windowTitle;
            this.authorizeResizing = authorizeResizing;
            this.authorizeALTF4 = authorizeALTF4;
            this.runInBackground = runInBackground;
            this.windowState = windowState;
        }

        public void CreateBoundingRectangle()
        {
            Vector2 v2 = Vector2.GetPosition(Vector2.Position.BottomLeft, new Rectangle(0, 0, renderedResolution.width, renderedResolution.height));
            boundingRectangle = new Rectangle((int)v2.X, (int)v2.Y, renderedResolution.width, renderedResolution.height);
            XNAboundningRectangle = boundingRectangle.rect;
        }

        internal void Initialize()
        {
            oldWindowResolution = windowResolution;
            
            CreateBoundingRectangle();

            Launcher.core.Activated += delegate (object sender, EventArgs e)
            {
                if (!runInBackground)
                    Launcher.coreStateBackground = Launcher.CoreState.Playing;

                WindowActivated?.Invoke(this, null);
            };

            Launcher.core.Deactivated += delegate (object sender, EventArgs e)
            {
                if(!runInBackground)
                    Launcher.coreStateBackground = Launcher.CoreState.Paused;

                WindowDeactivated?.Invoke(this, null);
            };

            Launcher.core.Exiting += delegate (object sender, EventArgs e)
            {
                Exiting?.Invoke(this, null);
            };

            Launcher.core.Window.FileDrop += delegate (object sender, FileDropEventArgs e)
            {
                FileDrop?.Invoke(e.Files);
            };

            Launcher.core.Window.ClientSizeChanged += delegate (object sender, EventArgs e)
            {
                windowResolution = new Size2(Launcher.core._graphics.PreferredBackBufferWidth, Launcher.core._graphics.PreferredBackBufferHeight);
                WindowResolutionChanged?.Invoke(windowResolution, oldWindowResolution);
                oldWindowResolution = windowResolution;
            };           

        }

        public void ApplyConfig()
        {
            if (Launcher.core == null) return;
            switch (windowState)
            {
                case WindowState.Windowed:
                    Launcher.core.Window.IsBorderless = false;
                    Launcher.core._graphics.IsFullScreen = false;
                    break;
                case WindowState.Fullscreen:
                    Launcher.core.Window.IsBorderless = false;
                    Launcher.core._graphics.HardwareModeSwitch = true;
                    Launcher.core._graphics.IsFullScreen = true;
                    break;
                case WindowState.Borderless:
                    Launcher.core.Window.IsBorderless = true;
                    Launcher.core._graphics.ToggleFullScreen();
                    break;
                case WindowState.FullscreenBorderless:
                    Launcher.core.Window.IsBorderless = true;
                    Launcher.core._graphics.HardwareModeSwitch = false;
                    Launcher.core._graphics.IsFullScreen = true;
                    break;
            }

            Launcher.core.Window.AllowAltF4 = authorizeALTF4;
            Launcher.core.Window.AllowUserResizing = authorizeResizing;
            Launcher.core.Window.Title = Launcher.core.Window.Title;

            Launcher.core._graphics.PreferredBackBufferWidth = (int)windowResolution.width;
            Launcher.core._graphics.PreferredBackBufferHeight = (int)windowResolution.height;
            Launcher.core._graphics.ApplyChanges();
        }
    }

}
