using boreal.engine.graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

namespace boreal.engine
{
    internal class Drawer : IDisposable
    {
        private List<DrawAction> drawActions = new List<DrawAction>();

        internal SpriteBatch sprites;

        private bool isDisposed;

        internal BasicEffect basicEffect;

        public EssentialDrawer essentialDrawer;
        public Shapes shapes;

        public Core core;
        public Camera camera;

        public RasterizerState rasterizerStateEmpty = new RasterizerState() { CullMode = CullMode.None };
        public RasterizerState rasterizerState = new RasterizerState() { MultiSampleAntiAlias = false, ScissorTestEnable = true, CullMode = CullMode.None };

        public Drawer(Core core)
        {
            sprites = new SpriteBatch(core.GraphicsDevice);

            essentialDrawer = new EssentialDrawer() { drawer = this };

            shapes = new Shapes(core) { spriteBatch = sprites };
            
            this.core = core;

            basicEffect = new BasicEffect(core.GraphicsDevice)
            {
                FogEnabled = false,
                TextureEnabled = true,
                LightingEnabled = false,
                VertexColorEnabled = true,
                Alpha = 1,
                World = Matrix.Identity,
                Projection = Matrix.Identity,
                View = Matrix.Identity,
            };
        }

        private int deltaScrollWheelValue = 0;
        private int currentScrollWheelValue = 0;

        public void Begin(Camera camera, bool isTextureFilteringEnabled, bool unset = false, SpriteSortMode spriteSortMode = SpriteSortMode.Deferred, BlendState blendState = null)
        {
            SamplerState samplerState = SamplerState.PointClamp;
            if (blendState == null)
                blendState = BlendState.AlphaBlend;

            var rasterizer = rasterizerStateEmpty;

            if (isTextureFilteringEnabled)
                samplerState = SamplerState.LinearClamp;

            if (camera == null)
            {
                Viewport vp = Launcher.core.GraphicsDevice.Viewport;
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0, 1);
                basicEffect.View = Matrix.Identity;
            }
            else
            {
                this.camera = camera;

                camera.UpdateMatrices();

                basicEffect.View = camera.view;
                basicEffect.Projection = camera.proj;
            }

            if (unset)
            {
                rasterizer = rasterizerState;
            }

            sprites.Begin(blendState: blendState, samplerState: samplerState, rasterizerState: rasterizer, effect: basicEffect, sortMode: spriteSortMode);
        }

        public void BeginUI()
        {
            sprites.Begin(rasterizerState: rasterizerState, effect: basicEffect, sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend);
        }

        private BlendState lightMapBlendState = new BlendState() { AlphaSourceBlend = Blend.Zero, AlphaDestinationBlend = Blend.InverseSourceColor, ColorSourceBlend = Blend.Zero, ColorDestinationBlend = Blend.InverseSourceColor };
        public void DrawLightMap(Lightmap lightmap, RenderTarget2D screen, RenderTarget2D lightMap, Microsoft.Xna.Framework.Rectangle destination, Camera cam)
        {
            core.GraphicsDevice.SetRenderTarget(lightMap);

            Begin(cam, true, false, SpriteSortMode.Immediate, lightMapBlendState);

            if(lightmap.ambientLight != null)
                Draw(lightmap.ambientLight.lightTexture.texture2D, null, cam.XNABoundingRectangle, Color.White);

            for (int i = 0; i < lightmap.lights.Count; i++)
            {
                Draw(lightmap.lights[i].lightTexture.texture2D, Microsoft.Xna.Framework.Vector2.Zero, lightmap.lights[i].gameObject.transform.position.xnaV2, Color.White);
            }

            sprites.End();

            core.GraphicsDevice.SetRenderTarget(null);

            sprites.Begin();
            sprites.Draw(screen, destination, Microsoft.Xna.Framework.Color.White);
            sprites.Draw(lightMap, destination, Microsoft.Xna.Framework.Color.White);
            sprites.End();

            //lights
            Begin(cam, true, false, SpriteSortMode.Immediate, BlendState.Additive);

            if (lightmap.ambientLight != null && lightmap.ambientLight.drawTexture)
                Draw(lightmap.ambientLight.lightTexture.texture2D, null, cam.XNABoundingRectangle, lightmap.ambientLight.color);

            for (int i = 0; i < lightmap.lights.Count; i++)
            {
                if (lightmap.lights[i].drawTexture)
                    Draw(lightmap.lights[i].lightTexture.texture2D, Microsoft.Xna.Framework.Vector2.Zero, lightmap.lights[i].gameObject.transform.position.xnaV2, lightmap.lights[i].color);
            }

            sprites.End();
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Microsoft.Xna.Framework.Vector2 origin, Microsoft.Xna.Framework.Vector2 position, Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int orderBy = 0)
        {
            CreateDrawAction(() => sprites.Draw(texture2D, position, null, color.color, 0f, origin, 1f, flip, 0), orderBy);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Microsoft.Xna.Framework.Rectangle? sourceRectangle, Microsoft.Xna.Framework.Vector2 origin, Microsoft.Xna.Framework.Vector2 position, float rotation, Microsoft.Xna.Framework.Vector2 scale, Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int orderBy = 0)
        {
            CreateDrawAction(() => sprites.Draw(texture2D, position, sourceRectangle, color.color, rotation, origin, scale, flip, 0), orderBy);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Microsoft.Xna.Framework.Rectangle? sourceRectangle, Microsoft.Xna.Framework.Rectangle destinationRectangle, Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int orderBy = 0)
        {
            CreateDrawAction(() => sprites.Draw(texture2D, destinationRectangle, sourceRectangle, color.color, 0f, Microsoft.Xna.Framework.Vector2.Zero, flip, 0), orderBy);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Microsoft.Xna.Framework.Rectangle destinationRectangle, Microsoft.Xna.Framework.Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int orderBy = 0)
        {
            CreateDrawAction(() => sprites.Draw(texture2D, destinationRectangle, null, color, 0, Microsoft.Xna.Framework.Vector2.Zero, flip, 0), orderBy);
        }

        public void DrawString(SpriteFont spriteFont, string text, Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int orderBy = 0, float scale = 1)
        {
            CreateDrawAction(() => sprites.DrawString(spriteFont, text, position, color, 0, Microsoft.Xna.Framework.Vector2.Zero, scale, flip, 0), orderBy);
        }

        public void DrawString(SpriteFont spriteFont, string text, Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Color color, float rotation, Microsoft.Xna.Framework.Vector2 origin, float scale, SpriteEffects flip = SpriteEffects.FlipVertically, int orderBy = 0)
        {
            sprites.DrawString(spriteFont, text, position, color, rotation, origin, scale, flip, 0);
        }

        public void SetScissorRectangle(Microsoft.Xna.Framework.Rectangle rect)
        {
            Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(
                rect.X, core.windowProfile.renderedResolution.height - rect.Y - rect.Height,
                rect.Width, rect.Height);

            core.GraphicsDevice.ScissorRectangle = r;
        }

        public void CreateDrawAction(Action action, int layer)
        {
            DrawAction drawAction = new DrawAction() { action = action, orderBy = layer };
            drawActions.Add(drawAction);
        }

        public void ResetScissorRectangle()
        {
            core.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, core.GraphicsDevice.Viewport.Width, core.GraphicsDevice.Viewport.Height);
        }

        public void End()
        {
            //shapes.End();

            ExecuteDrawActions();

            sprites.End();
        }

        public void ExecuteDrawActions()
        {
            drawActions = drawActions.OrderBy(x => x.orderBy).ToList();
            drawActions.Reverse();

            for (int i = 0; i < drawActions.Count; i++)
            {
                drawActions[i].action();
            }

            drawActions.Clear();
        }

        public void Dispose()
        {
            if (isDisposed) return;
            shapes.Dispose();
            sprites.Dispose();
            basicEffect.Dispose();
            isDisposed = true;
            drawActions.Clear();

            Console.WriteLine("sprites disposed");
        }

    }

    internal class DrawAction
    {
        public Action action;
        public int orderBy;
    }
}
