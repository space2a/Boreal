namespace boreal.engine
{
    internal static class ScreenErrors
    {

        public static void LoadingSceneNull(Sprites spritesBatch)
        {
            string text = "No loading scene, provide a scene to the SceneManager. ";
            Launcher.core.GraphicsDevice.Clear(Color.Black.color);
            spritesBatch.DrawString(FontManager.defaultFont.font,
                text,
                new Microsoft.Xna.Framework.Vector2(-FontManager.defaultFont.font.MeasureString(text).X / 2, 0),
                Microsoft.Xna.Framework.Color.Red);
        }

        public static void NoMainCamera(Sprites spritesBatch)
        {
            string text = "Missing a camera, set a camera in SceneLoader.currentScene.sceneCamera";
            Launcher.core.GraphicsDevice.Clear(Color.Black.color);
            spritesBatch.DrawString(FontManager.defaultFont.font,
            text,
                new Microsoft.Xna.Framework.Vector2(-FontManager.defaultFont.font.MeasureString(text).X / 2, 0),
                Microsoft.Xna.Framework.Color.Red);
        }

    }
}
