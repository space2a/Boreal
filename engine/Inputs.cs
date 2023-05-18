using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace boreal.engine
{
    public static class Inputs
    { 
        public static InputManager inputManager = new InputManager();

        public static KeyboardState KeyboardState = new KeyboardState();
        public static MouseState MouseState = new MouseState();

        public static bool suppressInputsWhenWindowIsInactive = true;

        internal static void Update()
        {
            if(SuppressInputs()) return;
            KeyboardState.Update();
            MouseState.Update();
        }

        internal static bool SuppressInputs() 
        {
            if(!suppressInputsWhenWindowIsInactive) return false;
            if (suppressInputsWhenWindowIsInactive) return !Launcher.core.IsActive;
            return false;
        }

    }

    public class KeyboardState
    {
        internal Microsoft.Xna.Framework.Input.KeyboardState previousState;
        internal Microsoft.Xna.Framework.Input.KeyboardState currentState;

        public bool isKeyboardBusy { get; internal set; } //set when doing something else than playing, ui textbox...
        public int isBusyFor = 0;


        public KeyboardState()
        {
            previousState = Keyboard.GetState();
            currentState = previousState;
        }

        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
            DoCycle();
        }

        internal void DoCycle()
        {
            if (isBusyFor > 0)
            {
                isBusyFor--;
                if (isBusyFor == 0) isKeyboardBusy = false;
            }
        }

        public bool SetBusyForXCycle(int numberofcycles)
        {
            if (numberofcycles == 1) numberofcycles = 2;
            if (numberofcycles < isBusyFor) return false;
            isKeyboardBusy = true;
            isBusyFor = numberofcycles;
            return true;
        }

        public bool GetKey(Keys key)
        {
            if (isKeyboardBusy) return false;
            return currentState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)(int)key) && !previousState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)(int)key);
        }

        public bool GetKeyDown(Keys key)
        {
            if (isKeyboardBusy) return false;
            return currentState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)(int)key);
        }

        public bool GetKeyUp(Keys key)
        {
            if (isKeyboardBusy) return false;
            return currentState.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)(int)key);
        }
    }

    public class MouseState
    {
        internal Microsoft.Xna.Framework.Input.MouseState previousState;
        internal Microsoft.Xna.Framework.Input.MouseState currentState;

        private MouseClicksInformation clicks = new MouseClicksInformation();
        private MouseClicksInformation clicksDown = new MouseClicksInformation();
        private MouseClicksInformation clicksUp = new MouseClicksInformation();

        public Vector2 worldMousePosition = Vector2.Zero;
        public Vector2 screenMousePosition = Vector2.Zero;
        public Vector2 windowMousePosition= Vector2.Zero;

        public Vector2 mouseDirection
        {
            get
            {
                return Vector2.Normalize(Vector2.Subtract(new Vector2(previousState.X, previousState.Y * -1), new Vector2(currentState.X, currentState.Y *-1)));
            }
        }

        public Vector2 mouseSpeed = Vector2.Zero;

        private int currentScrollWheelValue = 0;
        public int scrollWheelValue = 0;

        public bool lockCursor = false;
        public bool constrainCursor = false;
        public CursorContrainZone constrainCursorType = new CursorContrainWindow();

        public bool disableSafeLock = false;

        private bool ignoreCursorTeleportation = true;

        public bool isCursorVisible
        {
            get { return Launcher.core.IsMouseVisible; } 
            set { Launcher.core.IsMouseVisible = value; }
        }

        internal bool safeLock = true;

        private MouseButtons[] mouseButtons = new MouseButtons[5]
        {
            MouseButtons.LeftButton, MouseButtons.RightButton,
            MouseButtons.MiddleButton, MouseButtons.XButton1, MouseButtons.XButton2
        };

        internal void Update()
        {
            previousState = currentState;
            currentState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            scrollWheelValue = currentState.ScrollWheelValue - currentScrollWheelValue;
            currentScrollWheelValue += scrollWheelValue;

            if (lockCursor && safeLock && !disableSafeLock)
            {
                Mouse.SetPosition(Launcher.core.GraphicsDevice.Viewport.Width / 2,
                    Launcher.core.GraphicsDevice.Viewport.Height / 2);
                ignoreCursorTeleportation = true;
            }

            if (constrainCursor && constrainCursorType != null && safeLock && !disableSafeLock)
            {
                Rectangle constrainZone = constrainCursorType.GetRectangle();
                if (constrainZone != null)
                {
                    if (currentState.X < constrainZone.X - constrainZone.Width)
                        Mouse.SetPosition(constrainZone.X - constrainZone.Width, currentState.Y);
                    else if (currentState.X > constrainZone.X + constrainZone.Width)
                        Mouse.SetPosition(constrainZone.X + constrainZone.Width, currentState.Y);
                    else if (currentState.Y < constrainZone.Y - constrainZone.Height)
                        Mouse.SetPosition(currentState.X, constrainZone.Y - constrainZone.Height);
                    else if (currentState.Y > constrainZone.Y + constrainZone.Height)
                        Mouse.SetPosition(currentState.X, constrainZone.Y + constrainZone.Height);
                }
            }
            //Console.WriteLine("a" + previousState.X);

            ignoreCursorTeleportation = false;
            CacheClicks();
        }

        internal void UpdateScreenPosition(Screen screen, Camera camera)
        {
            Microsoft.Xna.Framework.Rectangle screenDestinationRectangle = screen.CalculateDestinationRectangle();
            Microsoft.Xna.Framework.Point windowPosition = currentState.Position;

            float sx = windowPosition.X - screenDestinationRectangle.X;
            float sy = (windowPosition.Y - screenDestinationRectangle.Y) * -1;
            sx /= screenDestinationRectangle.Width;
            sy /= screenDestinationRectangle.Height;

            sx *= screen.width;
            sy *= screen.height;

            camera.GetExtents(out float left, out float right, out float bottom, out float top);

            worldMousePosition = new Vector2(left + (sx / camera.zoom), (bottom + (sy / camera.zoom)) + (screen.height / camera.zoom));
            screenMousePosition = new Vector2(currentState.Position.X, currentState.Position.Y);
            windowMousePosition = new Vector2(left + sx, bottom + sy + screen.height);

            UIComponent.worldMouseRectangle = new Microsoft.Xna.Framework.Rectangle((int)worldMousePosition.X, (int)worldMousePosition.Y, 1, 1);

            //UIComponent.windowMouseRectangle = new Microsoft.Xna.Framework.Rectangle((int)(left + sx) + (screen.width /2), (int)(bottom + sy + screen.height + (screen.height /2)), 30, 30);

            UIComponent.windowMouseRectangle = new Microsoft.Xna.Framework.Rectangle(
            (int)(sx),
            (int)(sy + Launcher.core.windowProfile.renderedResolution.height),
            1, 1);
        }

        private void CacheClicks()
        {
            bool hasClicked = false;
            bool hasClickedD = false;
            bool hasClickedU = false;

            for (int i = 0; i < mouseButtons.Length; i++)
            {
                var st = GetButtonState(mouseButtons[i]);

                clicks.states[i] = st == MouseButtonState.Pressed;
                clicksDown.states[i] = st == MouseButtonState.Down;
                clicksUp.states[i] = st == MouseButtonState.Up;

                if (!hasClicked && st == MouseButtonState.Pressed) { hasClicked = true; }
                if (!hasClickedD && st == MouseButtonState.Down) { hasClickedD = true; }
                if (!hasClickedU && st == MouseButtonState.Up) { hasClickedU = true; }
            }

            clicks.hasAnyClick = hasClicked;
            clicksDown.hasAnyClick = hasClickedD;
            clicksUp.hasAnyClick = hasClickedU;
        }

        public bool GetButton(MouseButtons mouseButton)
        {
            return GetMouseButtonState(mouseButton, currentState) == ButtonState.Pressed && GetMouseButtonState(mouseButton, previousState) == ButtonState.Released;
        }
        public bool GetButtonDown(MouseButtons mouseButton)
        {
            return GetMouseButtonState(mouseButton, currentState) == ButtonState.Pressed;
        }

        public bool GetButtonUp(MouseButtons mouseButton)
        {
            return GetMouseButtonState(mouseButton, currentState) == ButtonState.Released && GetMouseButtonState(mouseButton, previousState) == ButtonState.Pressed;
        }

        public MouseButtonState GetButtonState(MouseButtons mouseButton)
        {
            if (GetButton(mouseButton)) return MouseButtonState.Pressed;
            else if (GetButtonDown(mouseButton)) return MouseButtonState.Down;
            else if (GetButtonUp(mouseButton)) return MouseButtonState.Up;
            else return MouseButtonState.None;
        }

        public MouseClicksInformation GetClicks () { return clicksUp; }
        public MouseClicksInformation GetClicksDown() { return clicksDown; }
        public MouseClicksInformation GetClicksUp() { return clicksUp; }

        internal ButtonState GetMouseButtonState(MouseButtons mouseButton, Microsoft.Xna.Framework.Input.MouseState state)
        {
            switch (mouseButton)
            {
                case MouseButtons.LeftButton:
                    return state.LeftButton;
                case MouseButtons.RightButton:
                    return state.RightButton;
                case MouseButtons.MiddleButton:
                    return state.MiddleButton;
                case MouseButtons.XButton1:
                    return state.XButton1;
                case MouseButtons.XButton2:
                    return state.XButton2;
            }
            return ButtonState.Released;
        }


    }

    public class InputManager
    {
        private List<Input> inputs = new List<Input>();

        public bool AddInput(Input input)
        {
            if(inputs.FindIndex(x => x.inputName == input.inputName) != -1) { return false; }
            inputs.Add(input);
            return true;
        }

        public bool IsInputDown(string name)
        {
            var input = GetInputByName(name);
            if (input == null) return false;

            for (int i = 0; i < input.inputSources.Length; i++)
            {
                if (input.inputSources[i] != null)
                {
                    bool r = false;
                    if ((input.inputSources[i] as KeyboardKeyInput) != null) { r = (input.inputSources[i] as KeyboardKeyInput).GetKeyDown(); if (r) return r; }
                    else if ((input.inputSources[i] as MouseButtonInput) != null) { r = (input.inputSources[i] as MouseButtonInput).GetButtonDown(); if (r) return r; }
                }
            }
            return false;
        }

        public bool IsInputUp(string name)
        {
            var input = GetInputByName(name);
            if (input == null) return false;

            for (int i = 0; i < input.inputSources.Length; i++)
            {
                if (input.inputSources[i] != null)
                {
                    bool r = false;
                    if ((input.inputSources[i] as KeyboardKeyInput) != null) { r = (input.inputSources[i] as KeyboardKeyInput).GetKeyUp(); if (r) return r; }
                    else if ((input.inputSources[i] as MouseButtonInput) != null) { r = (input.inputSources[i] as MouseButtonInput).GetButtonUp(); if (r) return r; }
                }
            }
            return false;
        }

        public Input GetInputByName(string name)
        {
            var i = inputs.FindIndex(x => x.inputName == name);
            if (i == -1) { return null; }
            return inputs[i];
        }
    }

    public class Input
    {
        public string inputName;
        public InputSource[] inputSources;

        public Input(string name, InputSource first) { inputName = name; inputSources = new InputSource[1]; inputSources[0] = first; }
        public Input(string name, InputSource first, InputSource second) { inputName = name; inputSources = new InputSource[2]; inputSources[0] = first; inputSources[1] = second; }
        public Input(string name, InputSource first, InputSource second, InputSource third) { inputName = name; inputSources = new InputSource[3]; inputSources[0] = first; inputSources[1] = second; inputSources[2] = third; }
    }

    public class InputSource
    {

    }

    public sealed class KeyboardKeyInput : InputSource
    {
        public Keys key { get; private set; }

        public KeyboardKeyInput(Keys key) { this.key = key; }

        public bool GetKeyDown() { if (Inputs.KeyboardState.isKeyboardBusy) return false; return Inputs.KeyboardState.GetKeyDown(key); }
        public bool GetKeyUp() { if (Inputs.KeyboardState.isKeyboardBusy) return false; return Inputs.KeyboardState.GetKeyUp(key); }
    }

    public sealed class MouseButtonInput : InputSource
    {
        public MouseButtons mouseButton { get; private set; }
        public bool GetButtonDown() 
        { return Inputs.MouseState.GetMouseButtonState(mouseButton, Inputs.MouseState.currentState) == ButtonState.Pressed; }
        public bool GetButtonUp()
        { return Inputs.MouseState.GetMouseButtonState(mouseButton, Inputs.MouseState.currentState) == ButtonState.Released; }

        public MouseButtonInput(MouseButtons mouseButton) { this.mouseButton = mouseButton; }
    }

    public sealed class MouseWheel : InputSource //To do...
    {

    }

    public sealed class ControllerButtonInput : InputSource //To do...
    {

    }

    public sealed class MouseClicksInformation
    {
        internal bool[] states = new bool[5];
        public bool hasAnyClick;
        public bool isLeft { get { return states[0]; } }
        public bool isRight { get { return states[1]; } }
        public bool isMiddle { get { return states[2]; } }
        public bool isXButton1 { get { return states[3]; } }
        public bool isButton2 { get { return states[4]; } }
    }

    public class CursorContrainZone
    {
        private Rectangle zone;
        public CursorContrainZone() { }
        public CursorContrainZone(Rectangle zone) { this.zone = zone; }

        public virtual Rectangle GetRectangle()
        {
            return zone;
        }
    }

    public sealed class CursorContrainWindow : CursorContrainZone
    {
        public override Rectangle GetRectangle()
        {
            return new Rectangle(Launcher.core.Window.Position.X + (Launcher.core._graphics.PreferredBackBufferWidth / 2),
                Launcher.core.Window.Position.Y + (Launcher.core._graphics.PreferredBackBufferHeight / 2),
                Launcher.core._graphics.PreferredBackBufferWidth, Launcher.core._graphics.PreferredBackBufferHeight);
        }

        public int edge = 30;

        public CursorContrainWindow(int edge = 30) { this.edge = edge; }
    }


    public enum MouseButtons
    {
        LeftButton = 0,
        RightButton = 1,
        MiddleButton = 2,
        XButton1 = 3,
        XButton2 = 4
    }

    public enum MouseButtonState
    {
        None,
        Pressed,
        Down,
        Up
    }

    public enum Keys
    {
        None = 0,
        Back = 8,
        Tab = 9,
        Enter = 13,
        CapsLock = 20,
        Escape = 27,
        Space = 0x20,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LeftWindows = 91,
        RightWindows = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 0x7F,
        F17 = 0x80,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LeftShift = 160,
        RightShift = 161,
        LeftControl = 162,
        RightControl = 163,
        LeftAlt = 164,
        RightAlt = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        OemPlus = 187,
        OemComma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        OemTilde = 192,
        OemOpenBrackets = 219,
        OemPipe = 220,
        OemCloseBrackets = 221,
        OemQuotes = 222,
        Oem8 = 223,
        OemBackslash = 226,
        ProcessKey = 229,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        Pa1 = 253,
        OemClear = 254,
        ChatPadGreen = 202,
        ChatPadOrange = 203,
        Pause = 19,
        ImeConvert = 28,
        ImeNoConvert = 29,
        Kana = 21,
        Kanji = 25,
        OemAuto = 243,
        OemCopy = 242,
        OemEnlW = 244
    }
}
